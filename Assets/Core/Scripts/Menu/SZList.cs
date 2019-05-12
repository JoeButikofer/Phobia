using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

class SZList : MonoBehaviour
{
    public GameObject listItemPrefab;

    protected List<string> safezonePaths;

    protected int selectionIndex = 0;
    protected string scenesFolderPath;

    // Use this for initialization
    protected virtual void Start()
    {
        scenesFolderPath = Path.Combine(Path.Combine("AssetBundles", BaseLoader.GetPlatformFolderForAssetBundles(Application.platform)), "safezones");

        safezonePaths = Directory.GetFiles(Path.Combine(Application.streamingAssetsPath, scenesFolderPath))
            .Where(file => file.Substring(file.Length - 8) != "manifest" && file.Substring(file.Length - 4) != "meta").ToList();

        int i = 0;
        foreach (var safezonePath in safezonePaths)
        {
            var sceneListItem = (GameObject)Instantiate(listItemPrefab, transform, false);

            var pathArray = safezonePath.Replace('\\', '/').Split('/');

            var safezoneName = pathArray[pathArray.Length - 1];

            sceneListItem.transform.Find("Text").GetComponent<Text>().text = safezoneName;

            sceneListItem.GetComponent<Button>().onClick.AddListener(() => { SelectSafezone(safezoneName); });
            i++;
        }
    }

    public void SelectSafezone(string SZName)
    {
        var safezone = DataService.Instance.GetPatientSafeZone(GlobalVariables.SelectedPatientId);
        safezone.Base = SZName;
        safezone.Objects = "";
        safezone.Music = "";

        DataService.Instance.UpdateSafezone(safezone);
    }
}
