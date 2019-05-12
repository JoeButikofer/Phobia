using UnityEngine;
using System.Collections;
using UnityEngine.VR;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using NewtonVR;
using System.IO;
using System.Linq;

public class SafezoneBuilder : MonoBehaviour {

    public SafezoneBuilderUI builderUI;
    public GameObject VRManagerPrefab;
    public GameObject EditorCamera;
    public Transform PlayerStartPosition;

	private float editorCamFoV = 60;

	private GameObject VRManager;
    private GameObject selectedPrefab;
    private Transform currentObject;
    private AudioSource audioSource;

    private AudioClip savedMusic;
    private List<GameObject> savedObjects;

    private bool isEraserEnabled = false;
    private bool isVREnabled = false;

	// Use this for initialization
	void Start () {

		if (UnityEngine.XR.XRSettings.enabled) {
			SwitchVRMode ();
		}

        audioSource = GetComponent<AudioSource>();

        if(savedObjects == null)
            savedObjects = new List<GameObject>();

        builderUI.FillObjectButtons(FindObjectsOfType<ObjectBundle>(), ChangeObject);
        builderUI.FillMusicButton(Resources.LoadAll<AudioClip>("SZMusic"), ChangeMusic);
    }

    public void SetMusic(AudioClip savedMusic)
    {
        this.savedMusic = savedMusic;
    }

    public void SetObjects(List<GameObject> savedObjects)
    {
        this.savedObjects = savedObjects;
    }

    void Update()
    {

        if(Input.GetMouseButtonDown(0))
        {
            bool isMouseOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1);

            if (!isMouseOverUI)
            {
                if (isEraserEnabled)
                {
                    //only collide with placed objects
                    int layerMask = 1 << LayerMask.NameToLayer("SZObject");

                    var distance = 100;
                    //create a ray cast and set it to the mouses cursor position in game
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, distance, layerMask))
                    {
                        savedObjects.Remove(hit.collider.transform.root.gameObject);
                        Destroy(hit.collider.transform.root.gameObject);
                    }
                }
                else if (currentObject != null)
                {
                    savedObjects.Add(currentObject.gameObject);

                    currentObject = null;
                    SpawnObject(selectedPrefab);
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape) && isVREnabled)
        {
            SwitchVRMode();
        }

        UpdateObjectPos();
    }
	
    void UpdateObjectPos()
    {
        if (currentObject != null)
        {
            //ignore SZ objects
            int layerMask = ~(1 << LayerMask.NameToLayer("SZObject"));

            var distance = 100;
            //create a ray cast and set it to the mouses cursor position in game
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, distance, layerMask))
            {
                currentObject.position = hit.point;
                currentObject.Rotate(Vector3.up, Input.mouseScrollDelta.y * 20);
            }
        }
    }

    public void ChangeObject(GameObject prefab)
    {
        isEraserEnabled = false;

        selectedPrefab = prefab;

        if (currentObject != null)
        {
            Destroy(currentObject.gameObject);
        }

        SpawnObject(selectedPrefab);
    }

    public void ChangeMusic(AudioClip music)
    {
        savedMusic = music;
        audioSource.clip = music;
        audioSource.Play();
    }

    private void SpawnObject(GameObject prefab)
    {
        if (prefab != null)
        {
            GameObject newObject = Instantiate(prefab);
            newObject.name = prefab.name; //Thanks to this, we can easily find the corresponding prefab at loading
            
            currentObject = newObject.transform;

            UpdateObjectPos();
        }
    }

	public void SwitchVRMode()
	{
		isVREnabled = !isVREnabled;
		builderUI.gameObject.SetActive(!isVREnabled);
		EditorCamera.SetActive(!isVREnabled);

		isEraserEnabled = false;
		currentObject = null;

		if (!isVREnabled)
		{
			if (VRManager != null) {
				Destroy (VRManager.gameObject);
				VRManager = null;
			}

			var pointer = FindObjectOfType<ParabolicPointer> ();
			if (pointer != null)
				Destroy (pointer.gameObject);
		}

		VRSwitch.instance.SwitchVRMode (isVREnabled, () => {

			VRManager = Instantiate(VRManagerPrefab);
			VRManager.transform.SetParent(this.transform);
			if(PlayerStartPosition != null)
			{
				VRManager.transform.position = PlayerStartPosition.position;
				VRManager.transform.rotation = PlayerStartPosition.rotation;
			}
		},
			() => {

				// TODO after disabling VR, the field of view of the camera is wrong
				// known issues : https://issuetracker.unity3d.com/issues/vr-toggling-off-vrsettings-dot-enabled-breaks-fov-in-standalone-player
				// but not fixed
				EditorCamera.GetComponent<Camera>().ResetFieldOfView();
				EditorCamera.GetComponent<Camera>().fieldOfView = editorCamFoV;
			});
	}


    public void SwitchEraserMode()
    {
        if (currentObject != null)
        {
            Destroy(currentObject.gameObject);
        }
        currentObject = null;

        isEraserEnabled = !isEraserEnabled;
    }

    public void Save()
    {
        string musicName = "";

        if (savedMusic != null)
            musicName = savedMusic.name;

        var sz = DataService.Instance.GetPatientSafeZone(GlobalVariables.SelectedPatientId);

        sz.Music = musicName;
        sz.Objects = SafezoneObjectToJSON.ObjectsToJson(savedObjects);

        DataService.Instance.UpdateSafezone(sz);
    }

    public void Quit()
    {
        SceneManager.LoadScene("mainMenu");
    }
}
