using UnityEngine;
using System.Collections;

public class FollowGroundPosition : MonoBehaviour {

    [SerializeField]
    private Transform objectToFollow;

	// Use this for initialization
	void Start ()
    {
        Follow();
    }
	
	// Update is called once per frame
	void Update ()
    {
        Follow();
    }

    void Follow()
    {
        var targetPos = objectToFollow.position;

        targetPos.y /= 2f;

        transform.position = targetPos;

        //var targetRot = objectToFollow.rotation;
        var targetForward = Vector3.ProjectOnPlane(objectToFollow.transform.forward, Vector3.up);

        transform.rotation = Quaternion.LookRotation(targetForward, Vector3.up);
    }
}
