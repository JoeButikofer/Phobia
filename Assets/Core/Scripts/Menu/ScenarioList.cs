using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.VR;

class ScenarioList : MonoBehaviour
{
    public GameObject listItemPrefab;
    public ScenarioDetailPanel detailPanel;

    protected List<string> sceneBundlePaths;
    protected List<ScenarioDetail> scenarioDetails;


    protected string selectedSceneName;
    protected string scenesFolderPath;

    // Use this for initialization
    void Start()
    {

        scenarioDetails = new List<ScenarioDetail>();

        scenesFolderPath = Path.Combine(Path.Combine("AssetBundles", BaseLoader.GetPlatformFolderForAssetBundles(Application.platform)), "scenario");

        sceneBundlePaths = Directory.GetFiles(Path.Combine(Application.streamingAssetsPath, scenesFolderPath))
            .Where(file => file.Substring(file.Length - 8) != "manifest" && file.Substring(file.Length - 4) != "meta").ToList();

        int i = 0;
        foreach (var sceneBundlePath in sceneBundlePaths)
        {
            var sceneListItem = (GameObject)Instantiate(listItemPrefab, transform, false);

            var path = sceneBundlePath.Replace('\\','/').Split('/');

            var sceneName = path[path.Length - 1];

            var detail = ScenarioParser.ParseScenario(sceneName);

            sceneListItem.transform.Find("Text").GetComponent<Text>().text = detail.Phobia + " : " + detail.Name;

            scenarioDetails.Add(detail);

            //The first scene is selected by default
            if (i == 0)
                selectedSceneName = sceneName;

            int currentIndex = i;
            sceneListItem.GetComponent<Button>().onClick.AddListener(() => {
                selectedSceneName = sceneName;
                detailPanel.ShowScenarioDetail(scenarioDetails[currentIndex]);
            });
            i++;
        }

        if(scenarioDetails.Count > 0)
            detailPanel.ShowScenarioDetail(scenarioDetails[0]);
    }

    public void LoadScene()
    {
        if (selectedSceneName != null)
        {
            detailPanel.CreateParameters(selectedSceneName);

            var SZMode = FindObjectOfType<SafezoneMode>();

            if (SZMode == null)
            {
                var obj = new GameObject();
                SZMode = obj.AddComponent<SafezoneMode>();
            }

            SZMode.IsBuilderMode = false;

			EnableVR ();

            StartCoroutine(AsyncLoad(selectedSceneName));
        }
    }

	private void EnableVR()
	{
		VRSwitch.instance.SwitchVRMode (true);
	}

    IEnumerator AsyncLoad(string sceneName)
    {

        yield return StartCoroutine(BaseLoader.Instance.LoadLevel("scenario/" + sceneName, sceneName, false));
        // Unload assetBundles.
        AssetBundleManager.UnloadAssetBundle("scenario/" + sceneName);
     
    }
}
