using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
class PauseDisplay : MonoBehaviour
{
    private TextMesh textMesh;

    void Start()
    {
        textMesh = GetComponent<TextMesh>();

        var scenarioManager = FindObjectOfType<ScenarioManager>();
        UpdateDisplay(scenarioManager.IsPaused);

        scenarioManager.pauseStateChanged += UpdateDisplay;
    }

    void UpdateDisplay(bool isPaused)
    {
        if(isPaused)
            textMesh.text = "Paused";
        else
            textMesh.text = "Play";
    }

}
