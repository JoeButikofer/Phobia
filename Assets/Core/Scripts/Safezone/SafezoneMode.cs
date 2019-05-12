using UnityEngine;
using System.Collections;

public class SafezoneMode : MonoBehaviour {

    public bool IsBuilderMode;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
