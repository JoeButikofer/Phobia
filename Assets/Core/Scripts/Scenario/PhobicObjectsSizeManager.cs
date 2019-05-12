using System.Collections.Generic;
using UnityEngine;

class PhobicObjectsSizeManager : MonoBehaviour
{
    public string sizeParameterName;
    public List<float> sizes;
    public GameObject phobicObjectsRoot;

    public int defaultValue;
    private float size;

    void Start()
    {
        SessionParameters parameters = FindObjectOfType<SessionParameters>();

        var sizeIndex = defaultValue;
        if (parameters != null)
        {
            sizeIndex = parameters.GetChoice(sizeParameterName, defaultValue).Index;  
        }

        if (sizes.Count > sizeIndex)
        {
            size = sizes[sizeIndex];

            UpdateAll();
        }
    }

    public void UpdateAll()
    {
        UpdateSingle(phobicObjectsRoot);
    }

    public void UpdateSingle(GameObject obj)
    {
        foreach (Transform go in obj.GetComponentsInChildren<Transform>(true))
        {
            if (go.CompareTag("PhobicObject"))
            {
                go.localScale = new Vector3(size, size, size);
            }
        }
    }
}
