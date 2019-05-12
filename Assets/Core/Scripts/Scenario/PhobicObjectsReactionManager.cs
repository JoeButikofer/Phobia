using System.Collections.Generic;
using UnityEngine;

class PhobicObjectsReactionManager : MonoBehaviour
{
    public string reactionParameterName;

    public ReactionType defaultValue;

    private ReactionType reaction;

    void Start()
    {
        SessionParameters parameters = FindObjectOfType<SessionParameters>();

        var speedIndex = defaultValue;
        if (parameters != null)
        { 
            reaction = (ReactionType)parameters.GetChoice(reactionParameterName, (int)defaultValue).Index;
        }
        else
        {
            reaction = defaultValue;
        }

        UpdateAll();
    }

    public void UpdateAll()
    {
        var phobicObjects = FindObjectsOfType<PhobicObjectController>();

        foreach (var phobicObject in phobicObjects)
        {
            phobicObject.reaction = reaction;
        }
    }

    public void UpdateSingle(GameObject obj)
    {
        foreach (var controller in obj.GetComponentsInChildren<PhobicObjectController>())
        {
            controller.reaction = reaction;
        }
    }
}

