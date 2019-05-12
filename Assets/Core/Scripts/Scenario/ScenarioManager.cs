using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Threading;

public delegate void PauseStateChanged(bool isPaused);

public class ScenarioManager : MonoBehaviour {

    public event PauseStateChanged pauseStateChanged;

    public bool StartPaused;

    [SerializeField]
    protected AudioSource audioSource;

    private bool isPaused;
    public bool IsPaused
    {
        get
        {
            return isPaused;
        }
    }

    protected SessionParameters parameters;

    private CommandParser parser;
    private EventProcessor eventProcessor;
    private NoteProcessor noteProcessor;
    private VideoRecorder videoRecorder;
    private bool sessionSaved = false;

    private Session session;

    protected virtual void Start()
    {
        // The event processor is used to execute action on the main thread
        eventProcessor = GetComponent<EventProcessor>();
        if (eventProcessor == null)
            eventProcessor = gameObject.AddComponent<EventProcessor>();

        // Init parser and corresponding events
        parser = new CommandParser();
        parser.playReceived += delegate () { eventProcessor.QueueEvent(new Action(Play)); };
        parser.pauseReceived += delegate () { eventProcessor.QueueEvent(new Action(Pause)); };
        parser.resetReceived += delegate () { eventProcessor.QueueEvent(new Action(Reset)); };
        parser.loadScenarioReceived += delegate () { eventProcessor.QueueEvent(new Action(LoadScenario)); };
        parser.loadSafezoneReceived += delegate () { eventProcessor.QueueEvent(new Action(LoadPatientSafezone)); };

        parameters = FindObjectOfType<SessionParameters>();

        if (parameters != null)
        {
            
            session = parameters.Session;

            // Init the note processor
            noteProcessor = new NoteProcessor(Application.persistentDataPath, session);
            parser.noteReceived += noteProcessor.Process;

            // Init the video recorder
            videoRecorder = GetComponent<VideoRecorder>();
            videoRecorder.session = session;
            videoRecorder.StartRecord();
        }



        // Transmit every command received directly to the parser
        CommunicationManager.Instance.commandReceived += parser.Parse;
        CommunicationManager.Instance.Start();

        if (StartPaused)
            Pause();
    }

    void OnDestroy()
    {
        if (!sessionSaved)
            SaveSession();

        CommunicationManager.Instance.commandReceived -= parser.Parse;
        CommunicationManager.Instance.Stop();
    }

    // Update is not affected by the time scale (pause, play)
    protected virtual void Update ()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!sessionSaved)
                SaveSession();

            LoadMainMenu();
        }

        // In cas the mobile app doesn't work
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }

            if (Input.GetKeyDown(KeyCode.P))
        {
            if (Time.timeScale > 0)
                Pause();
            else
                Play();
        }

        if(Input.GetKeyDown(KeyCode.G))
        {
            LoadPatientSafezone();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            LoadScenario();
        }
    }

    protected virtual void Pause()
    {
        if (audioSource != null)
            audioSource.Pause();

        isPaused = true;
        if (pauseStateChanged != null)
            pauseStateChanged(IsPaused);

        Time.timeScale = 0;
    }

    protected virtual void Play()
    {
        if(audioSource != null)
            audioSource.UnPause();

        isPaused = false;
        if (pauseStateChanged != null)
            pauseStateChanged(IsPaused);
        Time.timeScale = 1;
    }

    protected virtual void Reset()
    {
        sessionSaved = true; // This ScenarioManager is no longer responsible for saving this session
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    protected virtual void LoadPatientSafezone()
    {
        var szName = DataService.Instance.GetPatientSafeZone(GlobalVariables.SelectedPatientId);

        StartCoroutine(AsyncLoad("safezones", szName.Base));
    }

    protected virtual void LoadScenario()
    {
        StartCoroutine(AsyncLoad("scenario", parameters.ScenarioName));
    }

    protected void LoadMainMenu()
    {
        SceneManager.LoadScene("mainMenu");
    }

    IEnumerator AsyncLoad(string sceneFolder, string sceneName)
    {
        sessionSaved = true; // This ScenarioManager is no longer responsible for saving this session

        // Load level.
        yield return StartCoroutine(BaseLoader.Instance.LoadLevel(sceneFolder + "/" + sceneName, sceneName, false));

        // Unload assetBundles.
        AssetBundleManager.UnloadAssetBundle(sceneFolder + "/" + sceneName);
    }

    protected void SaveSession()
    {
        // Save session end time
        session.EndTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;

        DataService.Instance.UpdateSession(session);

        string absoluteVideoFolderPath = session.GetAbsoluteVideoPath(); //can only be called on main thread

        videoRecorder.EndRecord();
        new Thread(() => {

            ConvertCaptureToGif(absoluteVideoFolderPath);

        }).Start();

        sessionSaved = true;
    }

    private void ConvertCaptureToGif(string path)
    {
        string outputPath = Path.Combine(path, "capture.gif");

        session.Video = Path.Combine(session.Video, "capture.gif");
        DataService.Instance.UpdateSession(session);

        var gifWriter = new GifWriter(outputPath);

        // Find capture files
        var imageFiles = Directory.GetFiles(path).Where(file => file.Substring(file.Length - 4) == ".png").ToList();

        imageFiles.Sort();

        foreach (var imageFile in imageFiles)
        {
            var image = Image.FromFile(imageFile);
            gifWriter.WriteFrame(image);

            // Delete the file
            image.Dispose();
            File.Delete(imageFile);
        }

        gifWriter.Dispose();

    }
}
