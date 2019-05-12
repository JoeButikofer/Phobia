using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class Timeline : MonoBehaviour
    , IPointerClickHandler
    , IDragHandler
{

    public GifPlayer gifPlayer;
    public Transform Marker;
    public Transform NotesContainer;
    public GameObject TextNotePrefab;
    public GameObject ImageNotePrefab;

    private float progress;
    public float Progress {
        get
        {
            return progress;
        }
        private set
        {
            progress = Mathf.Clamp01(value);
            Marker.localPosition = new Vector3(rectTransform.rect.width * progress, Marker.localPosition.y, Marker.localPosition.z);
            gifPlayer.SetProgress(progress);
        }
    }

    private UILineDrawer mainLine;
    private RectTransform rectTransform;
    private Session session;
    private bool isPaused = true;
    private long notePreviewTime = 1 * 1000; // We want to see 1 or 2 sec before the note
    private List<Note> notes;
    private bool isEnabled = false;

    private int width;

    private Coroutine playCoroutineInstance = null;

    // Use this for initialization
    void Awake ()
    {
        mainLine = GetComponent<UILineDrawer>();
        rectTransform = this.GetComponent<RectTransform>();
        
    }

    void OnEnable()
    {
        isEnabled = true;
        session = DataService.Instance.GetSession(GlobalVariables.SelectedSessionId);
        notes = new List<Note>(DataService.Instance.GetSessionNotes(session.Id));

        gifPlayer.Init();
        gifPlayer.LoadGif(session.GetAbsoluteVideoPath());

        DrawTimeline();

        Progress = 0;
    }

    void OnDisable()
    {
        isEnabled = false;
        gifPlayer.Dispose();
        ClearTimeline();
        notes.Clear();
    }

    void OnRectTransformDimensionsChange()
    {
        DrawTimeline();
    }

    public void Play()
    {
        if (playCoroutineInstance != null)
            StopCoroutine(playCoroutineInstance);

        playCoroutineInstance = StartCoroutine(PlayCoroutine());
    }

    private IEnumerator PlayCoroutine(float maxProgress = 1)
    {
        var step = 1f / (gifPlayer.frameCount * 2);
        isPaused = false;

        while (!isPaused && Progress < maxProgress)
        {
            Progress += step;

            yield return new WaitForSecondsRealtime((gifPlayer.delay / 2f) / 1000f);
        }
    }

    public void Pause()
    {
        isPaused = true;
    }

    private void DrawTimeline()
    {
        if (isEnabled)
        {
            mainLine.P1 = new Vector2(rectTransform.rect.xMin, rectTransform.rect.center.y);
            mainLine.P2 = new Vector2(rectTransform.rect.xMax, rectTransform.rect.center.y);

            ClearTimeline();

            foreach (var note in notes)
            {
                DrawNoteOnTimeline(note);
                GenerateNote(note);
            }
        }
    }

    private void ClearTimeline()
    {
        foreach(Transform note in NotesContainer)
        {
            Destroy(note.gameObject);
        }
    }

    private void DrawNoteOnTimeline(Note note)
    {
        GameObject obj = new GameObject("NoteLine");
        obj.transform.SetParent(this.transform);

        var rectTransform = obj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0.5f);
        rectTransform.anchorMax = new Vector2(0, 0.5f);

        var line = obj.AddComponent<UILineDrawer>();
        line.LineWidth = 5;
        line.color = Color.yellow; 

        double startPosition = (note.StartTime - session.StartTime) / (double)(session.EndTime - session.StartTime);
        double endPosition = (note.EndTime - session.StartTime) / (double)(session.EndTime - session.StartTime);

        line.P1 = new Vector2(this.rectTransform.rect.xMin + (float)startPosition * this.rectTransform.rect.width, 0);
        line.P2 = new Vector2(this.rectTransform.rect.xMin + (float)endPosition * this.rectTransform.rect.width, 0);

        rectTransform.anchoredPosition = new Vector2(0, 0);
        obj.transform.SetParent(NotesContainer);
    }



    private void GenerateNote(Note note)
    {
        GameObject noteObj = null;

        switch (note.Type)
        {
            case "IMAGE":
                noteObj = Instantiate(ImageNotePrefab);

                var noteData = File.ReadAllBytes(note.GetAbsoluteDataPath());
                // Create a texture. Texture size does not matter, since
                // LoadImage will replace with with incoming image size.
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(noteData);

                noteObj.transform.Find("Image").GetComponent<RawImage>().texture = tex;

                break;
            case "TEXT":
                noteObj = Instantiate(TextNotePrefab);

                var noteText = File.ReadAllText(note.GetAbsoluteDataPath());

                noteObj.transform.GetComponentInChildren<Text>().text = noteText;
                noteObj.transform.GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = 1f;
                break;
        }

        var onTouch = noteObj.GetComponent<NoteMouseEvent>();
        onTouch.note = note;
        onTouch.onNoteTouched += ShowNote;

        long noteTime = note.StartTime + (note.EndTime - note.StartTime) / 2;

        double timelinePosition = (noteTime - session.StartTime) / (double)(session.EndTime - session.StartTime);

        noteObj.transform.SetParent(NotesContainer);
        noteObj.transform.localPosition = new Vector3((float)timelinePosition * rectTransform.rect.width, 0, 0);
        
        var noteLine = noteObj.transform.Find("Line").GetComponent<UILineDrawer>();
        noteLine.P1.y = -NotesContainer.GetComponent<RectTransform>().anchoredPosition.y - noteLine.GetComponent<RectTransform>().anchoredPosition.y;
    }

    private void ShowNote(Note note)
    {
        long start = note.StartTime - notePreviewTime;

        double startProgress = (start - session.StartTime) / (double)(session.EndTime - session.StartTime);

        Progress = (float)startProgress;

        double endProgress = (note.EndTime - session.StartTime) / (double)(session.EndTime - session.StartTime);

        if(playCoroutineInstance != null)
            StopCoroutine(playCoroutineInstance);

        playCoroutineInstance = StartCoroutine(PlayCoroutine((float)endProgress));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        MoveMarker(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveMarker(eventData.position);
    }

    void MoveMarker(Vector3 mousePosition)
    {
        var localPos = transform.InverseTransformPoint(mousePosition);

        Progress = localPos.x / rectTransform.rect.width;
    }
}
