using UnityEngine;
using System.Collections;
using UnityEngine.VR;

public class VRSeatedExperience : MonoBehaviour {

	public Transform Player;
	public Transform Head;

	public bool ResetWithSpaceKey = true;

	public bool ResetAfterStart = true;

	private float timeBeforeReset = 0.5f;

	// Use this for initialization
	void Start ()
    {
	   
            // Maybe a better solution but doesn't work in my case
			/* From : https://forum.unity3d.com/threads/openvr-how-to-reset-camera-properly.417509/#post-2792972
            Valve.VR.OpenVR.System.ResetSeatedZeroPose();
            Valve.VR.OpenVR.Compositor.SetTrackingSpace(Valve.VR.ETrackingUniverseOrigin.TrackingUniverseSeated);
			UnityEngine.VR.InputTracking.Recenter ();
			*/
			
		if (ResetAfterStart)
			StartCoroutine (WaitAndReset ());
	}


	IEnumerator WaitAndReset()
	{
		yield return new WaitForSecondsRealtime(timeBeforeReset);

		ResetPosition();

	}

	void Update()
	{

		if (ResetWithSpaceKey && Input.GetKeyDown (KeyCode.Space)) {

			ResetPosition ();
		}

	}


	void ResetPosition ()
	{
		var headLocalPosition = transform.InverseTransformPoint (Head.position);

		Player.localPosition = -headLocalPosition;
	}
}
