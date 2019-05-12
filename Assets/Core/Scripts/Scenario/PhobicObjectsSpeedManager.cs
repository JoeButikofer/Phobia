using System.Collections.Generic;
using UnityEngine;

class PhobicObjectsSpeedManager : MonoBehaviour
{
    public string speedParameterName;
    public List<float> speeds;

    public int defaultValue;
    private float speed;

    void Start()
    {
        SessionParameters parameters = FindObjectOfType<SessionParameters>();

        var speedIndex = defaultValue;
        if (parameters != null)
        {
            speedIndex = parameters.GetChoice(speedParameterName, defaultValue).Index;
        }

        if (speeds.Count > speedIndex)
        {
            speed = speeds[speedIndex];

            UpdateAll();
        }
    }

    public void UpdateAll()
    {
        var phobicObjects = FindObjectsOfType<PhobicObjectController>();

        foreach (var phobicObject in phobicObjects)
        {
            phobicObject.speed = speed;
        }
    }

    public void UpdateSingle(GameObject obj)
    {
        foreach (var controller in obj.GetComponentsInChildren<PhobicObjectController>())
        {
            controller.speed = speed;
        }
    }
}
