using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;

public class ScenarioDetailPanel : MonoBehaviour {

    public Text TextName;
    public Text TextDescription;
    public Dropdown DropdownIntensity;

    public GameObject BoolParameterPrefab;
    public GameObject IntParameterPrefab;
    public GameObject FloatParameterPrefab;
    public GameObject StringParameterPrefab;

    private Transform parametersPanel;

    private Dictionary<string, bool> boolParameters;
    private Dictionary<string, int> intParameters;
    private Dictionary<string, float> floatParameters;
    private Dictionary<string, Choice> choiceParameters;

    private ScenarioDetail detail;

    void Awake()
    {
        boolParameters = new Dictionary<string, bool>();
        intParameters = new Dictionary<string, int>();
        floatParameters = new Dictionary<string, float>();
        choiceParameters = new Dictionary<string, Choice>();

        parametersPanel = transform.Find("Parameters");
    }

    public void ShowScenarioDetail(ScenarioDetail detail)
    {
        ClearDetail();
        this.detail = detail;

        TextName.text = detail.Phobia + " : " + detail.Name;
        TextDescription.text = detail.Description;

        for (int i = detail.MinIntensityLevel; i <= detail.MaxIntensityLevel; i++)
        {
            DropdownIntensity.options.Add(new Dropdown.OptionData() { text = "Niveau " + i });
        }

        DropdownIntensity.onValueChanged.AddListener(
                        delegate (int value) {
                            OnIntensityChanged(value, detail);
                        }
                    );

        DropdownIntensity.value = 0;
        DropdownIntensity.RefreshShownValue();

        OnIntensityChanged(0, detail);
    }

    void ClearDetail()
    {
        DropdownIntensity.ClearOptions();
        ClearParameters();
    }

    void ClearParameters()
    {
        foreach (Transform child in parametersPanel)
        {
            Destroy(child.gameObject);
        }
    }

    void OnIntensityChanged(int intensityLevel, ScenarioDetail detail)
    {
        ClearParameters();

        foreach (var parameter in detail.Parameters)
        {
            if (parameter is BoolParameter)
            {
                var boolParameter = parameter as BoolParameter;
                var p = Instantiate(BoolParameterPrefab);
                string parameterName = parameter.Name;
                p.GetComponentInChildren<Text>().text = parameter.DisplayName;
                p.transform.SetParent(parametersPanel);
                var toggle = p.GetComponentInChildren<Toggle>();
                toggle.onValueChanged.AddListener(
                 delegate (bool value) { boolParameters[parameterName] = value; }
                );
                toggle.isOn = boolParameter.IntensityDefault[intensityLevel];
                boolParameters[parameterName] = boolParameter.IntensityDefault[intensityLevel];
            }
            else if (parameter is IntParameter)
            {
                var intParameter = parameter as IntParameter;
                var p = Instantiate(IntParameterPrefab);
                string parameterName = parameter.Name;
                p.GetComponentInChildren<Text>().text = parameter.DisplayName;

                p.transform.SetParent(parametersPanel);
                var slider = p.GetComponentInChildren<Slider>();
                slider.minValue = intParameter.Min;
                slider.maxValue = intParameter.Max;
                slider.onValueChanged.AddListener(
                 delegate (float value) { intParameters[parameterName] = (int)value; }
                );
                slider.value = intParameter.IntensityDefault[intensityLevel];
                intParameters[parameterName] = intParameter.IntensityDefault[intensityLevel];
            }
            else if (parameter is FloatParameter)
            {
                var floatParameter = parameter as FloatParameter;
                var p = Instantiate(FloatParameterPrefab);
                string parameterName = parameter.Name;
                p.GetComponentInChildren<Text>().text = parameter.DisplayName;

                p.transform.SetParent(parametersPanel);
                var slider = p.GetComponentInChildren<Slider>();
                slider.minValue = floatParameter.Min;
                slider.maxValue = floatParameter.Max;
                slider.onValueChanged.AddListener(
                 delegate (float value) { floatParameters[parameterName] = value; }
                );
                slider.value = floatParameter.IntensityDefault[intensityLevel];
                floatParameters[parameterName] = floatParameter.IntensityDefault[intensityLevel];
            }
            else if (parameter is ChoiceParameter)
            {
                var choiceParameter = parameter as ChoiceParameter;
                var p = Instantiate(StringParameterPrefab);
                string parameterName = parameter.Name;
                p.GetComponentInChildren<Text>().text = parameter.DisplayName;

                p.transform.SetParent(parametersPanel);

                var dropdown = p.GetComponentInChildren<Dropdown>();
                dropdown.ClearOptions();
                dropdown.AddOptions(choiceParameter.Values);

                dropdown.onValueChanged.AddListener(
                        delegate (int value) { choiceParameters[parameterName] = new Choice() { Text = dropdown.options[value].text, Index = value }; }
                    );

                choiceParameters[parameterName] = new Choice()
                {
                    Text = dropdown.options[choiceParameter.IntensityDefault[intensityLevel]].text,
                    Index = choiceParameter.IntensityDefault[intensityLevel]
                };
                dropdown.value = choiceParameter.IntensityDefault[intensityLevel];
            }
        }
    }

    public void CreateParameters(string scenarioName)
    {
        // Destroy old parameters if any
        var oldParameters = FindObjectOfType<SessionParameters>();
        if (oldParameters != null)
            Destroy(oldParameters.gameObject);

        var g = new GameObject();
        var sessionParameters = g.AddComponent<SessionParameters>();
        sessionParameters.ScenarioName = scenarioName;
        sessionParameters.Session = CreateSession();

        sessionParameters.IntParameters = intParameters;
        sessionParameters.FloatParameters = floatParameters;
        sessionParameters.ChoiceParameters = choiceParameters;
        sessionParameters.BoolParameters = boolParameters;
    }

    private Session CreateSession()
    {
        long timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;

        var patient = DataService.Instance.GetPatient(GlobalVariables.SelectedPatientId);
        string patientFolderName = patient.Name + patient.Surname + patient.Id;

        string sessionFolderName = EpochTools.ConvertEpochToSortableDateTimeString(timestamp);

        string videoPath = Path.Combine(Path.Combine("capture", patientFolderName), sessionFolderName);

        Session s = new Session
        {
            StartTime = timestamp,
            PatientId = GlobalVariables.SelectedPatientId,
            ScenarioName = detail.Phobia + " : " + detail.Name,
            Video = videoPath
        };

        DataService.Instance.CreateSession(s);

        return s;
    }
}
