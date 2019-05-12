using UnityEngine;
using System.Collections;

public class ExtendedFlycam : MonoBehaviour
{

    /*
	EXTENDED FLYCAM
		Desi Quintans (CowfaceGames.com), 17 August 2012.
		Based on FlyThrough.js by Slin (http://wiki.unity3d.com/index.php/FlyThrough), 17 May 2011.
 
	LICENSE
		Free as in speech, and free as in beer.
 
	FEATURES
		WASD/Arrows:    Movement
		          Q:    Climb
		          E:    Drop
                      Shift:    Move faster
                    Control:    Move slower
                        End:    Toggle cursor locking to screen (you can also press Ctrl+P to toggle play mode on and off).
	*/

    public float cameraSensitivity = 90;
    public float climbSpeed = 4;
    public float normalMoveSpeed = 10;
    public float slowMoveFactor = 0.25f;
    public float fastMoveFactor = 3;
    public bool cameraRotationWhenRightMouseDown;


    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;

        rotationX += Input.GetAxisRaw("Mouse X") * cameraSensitivity * Time.unscaledDeltaTime;
        rotationY += Input.GetAxisRaw("Mouse Y") * cameraSensitivity * Time.unscaledDeltaTime;
        rotationY = Mathf.Clamp(rotationY, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
    }

    void Update()
    {
        if (Input.GetMouseButton(1) || !cameraRotationWhenRightMouseDown)
        {
            rotationX += Input.GetAxisRaw("Mouse X") * cameraSensitivity * Time.unscaledDeltaTime;
            rotationY += Input.GetAxisRaw("Mouse Y") * cameraSensitivity * Time.unscaledDeltaTime;
            rotationY = Mathf.Clamp(rotationY, -90, 90);

            transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
            transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxisRaw("Vertical") * Time.unscaledDeltaTime;
            transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxisRaw("Horizontal") * Time.unscaledDeltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxisRaw("Vertical") * Time.unscaledDeltaTime;
            transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxisRaw("Horizontal") * Time.unscaledDeltaTime;
        }
        else
        {
            transform.position += transform.forward * normalMoveSpeed * Input.GetAxisRaw("Vertical") * Time.unscaledDeltaTime;
            transform.position += transform.right * normalMoveSpeed * Input.GetAxisRaw("Horizontal") * Time.unscaledDeltaTime;
        }


        if (Input.GetKey(KeyCode.Q)) { transform.position += transform.up * climbSpeed * Time.unscaledDeltaTime; }
        if (Input.GetKey(KeyCode.E)) { transform.position -= transform.up * climbSpeed * Time.unscaledDeltaTime; }

        if (Input.GetKeyDown(KeyCode.End))
        {
            Cursor.lockState = (Cursor.lockState == CursorLockMode.Confined) ? CursorLockMode.None : CursorLockMode.Confined;
        }
    }
}