using UnityEngine;
using SimpleJSON;
using System.Collections.Generic;
using System.Collections;

public class SafezoneManager : MonoBehaviour {

    public GameObject ScenarioManagerPrefab;
    public GameObject BuilderPrefab;
    public GameObject PlayerPrefab;
    public Transform PlayerStartPosition;
    public bool DefaultModeIsBuilderMode;

    private AudioSource audioSource;

    private SafezoneBuilder builder;

    // Use this for initialization
    void Start() {

        var SZMode = FindObjectOfType<SafezoneMode>();
        var builderMode = DefaultModeIsBuilderMode;

        if (SZMode != null)
        {
            builderMode = SZMode.IsBuilderMode;
        }

        if (builderMode)
        {
            CreateBuilder();
        }
        else
        {
            CreateScenarioManager();
        }

        Safezone sz = DataService.Instance.GetPatientSafeZone(GlobalVariables.SelectedPatientId);
        StartCoroutine(LoadObjects(sz.Objects, builderMode));
        StartCoroutine(LoadMusic(sz.Music, builderMode));
    }

    private void CreateScenarioManager()
    {
        var scenarioManager = Instantiate(ScenarioManagerPrefab);
        audioSource = scenarioManager.GetComponent<AudioSource>();

        var player = Instantiate(PlayerPrefab);
        if (PlayerStartPosition != null)
        {
            player.transform.position = PlayerStartPosition.position;
            player.transform.rotation = PlayerStartPosition.rotation;
        }
    }

    private void CreateBuilder()
    {
        var builderObj = Instantiate(BuilderPrefab);
        builder = builderObj.GetComponent<SafezoneBuilder>();
        audioSource = builderObj.GetComponent<AudioSource>();

        if (PlayerStartPosition != null)
        {
            var camera = builderObj.transform.Find("Main Camera");
            camera.position = PlayerStartPosition.position + Vector3.up * 2;
            camera.rotation = PlayerStartPosition.rotation;

            builder.PlayerStartPosition = PlayerStartPosition;
        }
    }

    private IEnumerator LoadMusic(string musicName, bool builderMode)
    {
        
        var request =  Resources.LoadAsync<AudioClip>("SZMusic/" + musicName);

        // Wait until completion
        while (!request.isDone)
        {
            yield return null;
        }

        AudioClip music = request.asset as AudioClip;
        audioSource.clip = music;
        audioSource.volume = 0.5f;
        audioSource.Play();

        if (builderMode)
        {
            builder.SetMusic(music);
        }
    }

    private IEnumerator LoadObjects(string objectsJson, bool builderMode)
    {
        var parsedObjects = JSON.Parse(objectsJson);

        List<GameObject> objects = new List<GameObject>();

        if (parsedObjects != null)
        {
            for (int i = 0; i < parsedObjects.Count; i++)
            {
                var request = Resources.LoadAsync<GameObject>("SZPrefab/" + parsedObjects[i]["name"].Value);

                // Wait until completion
                while (!request.isDone)
                {
                    yield return null;
                }
                GameObject prefab = request.asset as GameObject;

                var loadedObj = Instantiate(prefab);
                loadedObj.name = prefab.name; //Thanks to this, we can easily find the corresponding prefab at loading
                loadedObj.transform.position = ParseVector3(parsedObjects[i]["transform"]["position"]);
                loadedObj.transform.rotation = ParseQuaternion(parsedObjects[i]["transform"]["rotation"]);
                loadedObj.transform.localScale = ParseVector3(parsedObjects[i]["transform"]["scale"]);

                objects.Add(loadedObj);
            }
        }

        if (builderMode)
        {
            builder.SetObjects(objects);
        }
    }

    private Vector3 ParseVector3(JSONNode vecJson)
    {
        return new Vector3(vecJson["x"].AsFloat, vecJson["y"].AsFloat, vecJson["z"].AsFloat);
    }

    private Quaternion ParseQuaternion(JSONNode quatJson)
    {
        return new Quaternion(quatJson["x"].AsFloat, quatJson["y"].AsFloat, quatJson["z"].AsFloat, quatJson["w"].AsFloat);
    }

}
