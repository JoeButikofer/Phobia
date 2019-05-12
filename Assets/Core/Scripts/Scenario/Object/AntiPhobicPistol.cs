using UnityEngine;
using System.Collections;
using NewtonVR;

public class AntiPhobicPistol : MonoBehaviour {

    public Material laserMaterial;
    public Color onColor;
    public Color offColor;
    public Transform FirePoint;

    public NVRHand Hand;

    private Transform laser;

    // Use this for initialization
    void Start ()
    {
        GameObject laserObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        laserObj.transform.SetParent(this.transform);
        laserObj.transform.rotation = Quaternion.AngleAxis(90, transform.right);
        laserObj.GetComponent<MeshRenderer>().material = laserMaterial;
        laserObj.GetComponent<MeshRenderer>().material.color = offColor;

        Destroy(laserObj.GetComponent<Collider>());

        laser = laserObj.transform;
    }

    // Update is called once per frame
    void Update()
    {

        float distance = 1000;

        Ray ray = new Ray(FirePoint.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance))
        {
            distance = Vector3.Distance(hit.point, FirePoint.position);
        }

        laser.localScale = new Vector3(0.01f, distance / 2f, 0.01f);
        laser.position = FirePoint.position + transform.forward * distance / 2f;

        if (Hand.Inputs[NVRButtons.Trigger].IsPressed)
        {
            if (Physics.Raycast(FirePoint.position, transform.forward, out hit, distance))
            {
                if (hit.collider.CompareTag("PhobicObject"))
                {
                    Destroy(hit.transform.gameObject);
                }
            }

            Hand.TriggerHapticPulse(500);
        }

        if (Hand.Inputs[NVRButtons.Trigger].PressDown == true)
        {
            laser.GetComponent<MeshRenderer>().material.color = onColor;
        }

        if (Hand.Inputs[NVRButtons.Trigger].PressUp == true)
        {
            laser.GetComponent<MeshRenderer>().material.color = offColor;
        }
    }
}
