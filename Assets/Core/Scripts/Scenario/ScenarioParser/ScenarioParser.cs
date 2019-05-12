using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScenarioParser
{

    public static ScenarioDetail ParseScenario(string scenarioName)
    {
        ScenarioDetail detail = new ScenarioDetail();

        var json = LoadJsonFromFile(scenarioName);
        var scenarioDetailJson = JSON.Parse(json);

        detail.Name = scenarioDetailJson["name"];
        detail.Phobia = scenarioDetailJson["phobia"];
        detail.Description = scenarioDetailJson["description"];

        detail.MinIntensityLevel = scenarioDetailJson["minIntensityLevel"].AsInt;
        detail.MaxIntensityLevel = scenarioDetailJson["maxIntensityLevel"].AsInt;

        detail.Parameters = ParseParameters(scenarioDetailJson["parameters"].AsArray);

        return detail;
    }

    static List<Parameter> ParseParameters(JSONArray parametersArray)
    {
        var parameters = new List<Parameter>();

        for (int i = 0; i < parametersArray.Count; i++)
        {
            var parameterJson = parametersArray[i];

            switch (parameterJson["type"])
            {
                case "bool":
                    BoolParameter pBool = new BoolParameter();
                    pBool.Name = parameterJson["name"].Value;
                    pBool.DisplayName = parameterJson["displayName"].Value;
                    pBool.IntensityDefault = ParseBoolValues(parameterJson["intensityDefault"].AsArray);
                    parameters.Add(pBool);
                    break;
                case "int":
                    IntParameter pInt = new IntParameter();
                    pInt.Min = parameterJson["min"].AsInt;
                    pInt.Max = parameterJson["max"].AsInt;
                    pInt.Name = parameterJson["name"].Value;
                    pInt.DisplayName = parameterJson["displayName"].Value;
                    pInt.IntensityDefault = ParseIntValues(parameterJson["intensityDefault"].AsArray);
                    parameters.Add(pInt);
                    break;
                case "float":
                    FloatParameter pFloat = new FloatParameter();
                    pFloat.Min = parameterJson["min"].AsFloat;
                    pFloat.Max = parameterJson["max"].AsFloat;
                    pFloat.Name = parameterJson["name"].Value;
                    pFloat.DisplayName = parameterJson["displayName"].Value;
                    pFloat.IntensityDefault = ParseFloatValues(parameterJson["intensityDefault"].AsArray);
                    parameters.Add(pFloat);
                    break;
                case "choice":
                    ChoiceParameter pString = new ChoiceParameter();
                    pString.Values = ParseStringValues(parameterJson["values"].AsArray);
                    pString.Name = parameterJson["name"].Value;
                    pString.DisplayName = parameterJson["displayName"].Value;
                    pString.IntensityDefault = ParseIntValues(parameterJson["intensityDefault"].AsArray);
                    parameters.Add(pString);
                    break;
            }
        }

        return parameters;
    }

    static List<string> ParseStringValues(JSONArray array)
    {
        List<string> values = new List<string>();

        for (int i = 0; i < array.Count; i++)
        {
            values.Add(array[i].Value);
        }

        return values;
    }

    static List<int> ParseIntValues(JSONArray array)
    {
        List<int> values = new List<int>();

        for (int i = 0; i < array.Count; i++)
        {
            values.Add(array[i].AsInt);
        }

        return values;
    }

    static List<float> ParseFloatValues(JSONArray array)
    {
        List<float> values = new List<float>();

        for (int i = 0; i < array.Count; i++)
        {
            values.Add(array[i].AsFloat);
        }

        return values;
    }

    static List<bool> ParseBoolValues(JSONArray array)
    {
        List<bool> values = new List<bool>();

        for (int i = 0; i < array.Count; i++)
        {
            values.Add(array[i].AsBool);
        }

        return values;
    }

    static string LoadJsonFromFile(string scenarioName)
    {
        TextAsset jsonFile = Resources.Load("ScenarioDetails/" + scenarioName) as TextAsset;
        return jsonFile.text;
    }
}
