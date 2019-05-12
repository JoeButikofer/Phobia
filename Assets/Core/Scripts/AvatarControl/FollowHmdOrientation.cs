using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.VR;

class FollowHmdOrientation : MonoBehaviour
{
    void Update()
    {
        var headRotation = UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.Head);
        this.transform.localRotation = new Quaternion(headRotation.x, headRotation.y, headRotation.z, headRotation.w);
    }
}
