using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;

public class SafezoneObjectToJSON
{

    public static string ObjectsToJson(List<GameObject> objects)
    {
        var json = new JSONArray();

        foreach (var obj in objects)
        {
            var objJson = new JSONClass();

            objJson["name"] = obj.name;
            objJson["transform"] = TransformToJson(obj.transform);

            json.Add(objJson);
        }

        return json.ToString();
    }

    public static JSONNode TransformToJson(Transform trans)
    {
        var transJson = new JSONClass();

        transJson["position"] = Vector3ToJson(trans.position);
        transJson["rotation"] = QuaternionToJson(trans.rotation);
        transJson["scale"] = Vector3ToJson(trans.localScale);

        return transJson;
    }

    public static JSONNode Vector3ToJson(Vector3 vector)
    {
        var vecJson = new JSONClass();

        vecJson["x"] = vector.x.ToString();
        vecJson["y"] = vector.y.ToString();
        vecJson["z"] = vector.z.ToString();

        return vecJson;
    }

    public static JSONNode QuaternionToJson(Quaternion quat)
    {
        var quatJson = new JSONClass();

        quatJson["x"] = quat.x.ToString();
        quatJson["y"] = quat.y.ToString();
        quatJson["z"] = quat.z.ToString();
        quatJson["w"] = quat.w.ToString();

        return quatJson;
    }

}
