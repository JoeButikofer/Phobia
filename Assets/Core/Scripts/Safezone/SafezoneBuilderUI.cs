using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class SafezoneBuilderUI : MonoBehaviour {

    public Transform bundleLayout;
    public Transform musicLayout;

    public GameObject bundlePanelPrefab;
    public GameObject objectButtonPrefab;

    public GameObject musicButtonPrefab;

    public Button saveBtn;
    public Button quitBtn;

    // Use this for initialization
    public void Init(SafezoneBuilder safezoneBuilder)
    {
        saveBtn.onClick.AddListener(() => safezoneBuilder.Save());
        quitBtn.onClick.AddListener(() => safezoneBuilder.Quit());
    }

    public void FillObjectButtons(ObjectBundle[] bundles, Action<GameObject> buttonCallback)
    {
        foreach(var bundle in bundles)
        {
            CreateBundlePanel(bundle, buttonCallback);
        }
    }

    void CreateBundlePanel(ObjectBundle bundle, Action<GameObject> buttonCallback)
    {

        var bundlePanel = Instantiate(bundlePanelPrefab);
        bundlePanel.transform.SetParent(bundleLayout);
        bundlePanel.transform.Find("Text").GetComponent<Text>().text = bundle.bundleName;

        var layout = bundlePanel.transform.Find("ObjectLayout");

        foreach (var bundleObject in bundle.objects)
        {
            var description = bundleObject.GetComponent<ObjectDescription>();

            var objectButton = Instantiate(objectButtonPrefab);
            objectButton.transform.SetParent(layout);

            objectButton.transform.Find("Image").GetComponent<Image>().sprite = description.objectImage;
            
            GameObject cur = bundleObject; // local variable for callback
            objectButton.GetComponent<Button>().onClick.AddListener(() => buttonCallback(cur));
        }
    }

    public void FillMusicButton(AudioClip[] musics, Action<AudioClip> buttonCallback)
    {
        foreach (var music in musics)
        {
            var musicButton = Instantiate(musicButtonPrefab);
            musicButton.transform.SetParent(musicLayout);

            musicButton.transform.Find("Text").GetComponent<Text>().text = music.name;

            AudioClip cur = music; // local variable for callback
            musicButton.GetComponent<Button>().onClick.AddListener(() => buttonCallback(cur));
        }

        // Mute button

        var muteButton = Instantiate(musicButtonPrefab);
        muteButton.transform.SetParent(musicLayout);

        muteButton.transform.Find("Text").GetComponent<Text>().text = "Rien";
        muteButton.GetComponent<Button>().onClick.AddListener(() => buttonCallback(null));
    }
}
