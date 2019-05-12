using UnityEngine;
using System.Collections.Generic;

public class ObjectScenarioManager : ScenarioManager {

    public List<GameObject> objectsForParameter;
    public string parameterName;

    public int defaultValue;

    protected override void Start()
    {
        base.Start();

        int level = defaultValue;

        if (parameters != null)
        {
            level = parameters.GetInt(parameterName, defaultValue);
        }

        for (int i = 0; i < objectsForParameter.Count; i++)
        {
            if (objectsForParameter[i] != null)
                objectsForParameter[i].SetActive(i == level);
        }
    }
}
