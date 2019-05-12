using System;
using UnityEngine;
using UnityEngine.VR;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class VRSwitch : MonoBehaviour
{

	public static VRSwitch instance; // Singleton

	private List<String> supportedVRDevices;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad(gameObject);

		supportedVRDevices = UnityEngine.XR.XRSettings.supportedDevices.ToList();
        supportedVRDevices = supportedVRDevices.Where(dev => dev != "None").ToList(); 
        supportedVRDevices.Add("None"); // The "None" device must be at the end
    }

	public void SwitchVRMode(bool isVREnabled, Action enabledAction = null, Action disabledAction = null)
	{
		if (isVREnabled) {
			StartCoroutine (LoadDevice (supportedVRDevices.ToArray(), enabledAction, disabledAction));
		} else {
			StartCoroutine (LoadDevice (new string[]{""}, enabledAction, disabledAction));
		}
	}

	IEnumerator LoadDevice(string[] devices, Action enabledAction, Action disabledAction) {
		UnityEngine.XR.XRSettings.LoadDeviceByName(devices);
		yield return null;
		if (UnityEngine.XR.XRSettings.loadedDeviceName != "") {
			UnityEngine.XR.XRSettings.enabled = true;
			Debug.Log ("VRSwitch : VR enabled");

			SteamVR.enabled = true;

			if (enabledAction != null)
				enabledAction ();

		} else {
			Debug.Log ("VRSwitch : VR disabled");

			UnityEngine.XR.XRSettings.enabled = false;

			if (disabledAction != null)
				disabledAction ();
		}
	}

}


