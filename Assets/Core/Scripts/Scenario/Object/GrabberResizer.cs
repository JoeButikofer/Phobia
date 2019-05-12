using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Grabber))]
public class GrabberResizer : MonoBehaviour {

    public string parameterName;
    public List<float> lengths;
    public int defaultValue;

    // Use this for initialization
    void Start ()
    {
        SessionParameters parameters = FindObjectOfType<SessionParameters>();

        var lengthIndex = defaultValue;
        if (parameters != null)
        {
            lengthIndex = parameters.GetChoice(parameterName, defaultValue).Index;
        }

        if (lengths.Count > lengthIndex)
        {
            GetComponent<Grabber>().MaxLength = lengths[lengthIndex];
        }
    }
}
