using UnityEngine;
using System.Collections;

public class SZMenu : MonoBehaviour {

    public void CustomizeSZ()
    {
        var safezone = DataService.Instance.GetPatientSafeZone(GlobalVariables.SelectedPatientId);

        var SZMode = FindObjectOfType<SafezoneMode>();

        if(SZMode == null)
        {
            var obj = new GameObject();
            SZMode = obj.AddComponent<SafezoneMode>();
        }

        SZMode.IsBuilderMode = true;

        StartCoroutine(AsyncLoad(safezone.Base));
    }

    IEnumerator AsyncLoad(string sceneName)
    {
        // Load level.
        yield return StartCoroutine(BaseLoader.Instance.LoadLevel("safezones/" + sceneName, sceneName, false));

        // Unload assetBundles.
        AssetBundleManager.UnloadAssetBundle("safezones/" + sceneName);
    }
}
