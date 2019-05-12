using UnityEngine;
using System.Collections.Generic;

public class SessionParameters : MonoBehaviour {


    public string ScenarioName { get; set; }
    public Session Session { get; set; }
    public Dictionary<string, bool> BoolParameters { get; set; }
    public Dictionary<string, int> IntParameters { get; set; }
    public Dictionary<string, float> FloatParameters { get; set; }
    public Dictionary<string, Choice> ChoiceParameters { get; set; }

    void Awake()
    {
        BoolParameters = new Dictionary<string, bool>();
        IntParameters = new Dictionary<string, int>();
        FloatParameters = new Dictionary<string, float>();
        ChoiceParameters = new Dictionary<string, Choice>();

        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Try to retrieve a bool from parameters, return the default value if the parameter isn't present
    /// </summary>
    /// <param name="parameterName">name of the parameter</param>
    /// <param name="defaultValue">the value returned if the parameter isn't present</param>
    /// <returns>the parameter if present, the default value otherwise</returns>
    public bool GetBool(string parameterName, bool defaultValue = false)
    {
        return BoolParameters.ContainsKey(parameterName) ? BoolParameters[parameterName] : defaultValue;
    }

    public void SetBool(string parameterName, bool value)
    {
        BoolParameters[parameterName] = value;
    }

    /// <summary>
    /// Try to retrieve an int from parameters, return the default value if the parameter isn't present
    /// </summary>
    /// <param name="parameterName">name of the parameter</param>
    /// <param name="defaultValue">the value returned if the parameter isn't present</param>
    /// <param name="tryChoice">If we must check choice parameters in case the int parameter isn't present</param>
    /// <returns>the parameter if present, the default value otherwise</returns>
    public int GetInt(string parameterName, int defaultValue = 0, bool tryChoice = false)
    {
        if(tryChoice)
            return IntParameters.ContainsKey(parameterName) ? IntParameters[parameterName] : GetChoice(parameterName, defaultValue).Index;
        else
            return IntParameters.ContainsKey(parameterName) ? IntParameters[parameterName] : defaultValue;
    }

    public void SetInt(string parameterName, int value)
    {
        IntParameters[parameterName] = value;
    }

    /// <summary>
    /// Try to retrieve a float from parameters, return the default value if the parameter isn't present
    /// </summary>
    /// <param name="parameterName">name of the parameter</param>
    /// <param name="defaultValue">the value returned if the parameter isn't present</param>
    /// <returns>the parameter if present, the default value otherwise</returns>
    public float GetFloat(string parameterName, float defaultValue = 0)
    {
        return FloatParameters.ContainsKey(parameterName) ? FloatParameters[parameterName] : defaultValue;
    }

    public void SetFloat(string parameterName, float value)
    {
        FloatParameters[parameterName] = value;
    }

    /// <summary>
    /// Try to retrieve a choice (text, index) from parameters, return the default value if the parameter isn't present
    /// </summary>
    /// <param name="parameterName">name of the parameter</param>
    /// <param name="defaultValue">the value returned if the parameter isn't present</param>
    /// <returns>the parameter if present, the default value otherwise</returns>
    public Choice GetChoice(string parameterName, int defaultValue = 0)
    {
        return ChoiceParameters.ContainsKey(parameterName) ? ChoiceParameters[parameterName] : new Choice() { Index=defaultValue, Text="default" };
    }
}
