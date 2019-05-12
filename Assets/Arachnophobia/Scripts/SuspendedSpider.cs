using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class SuspendedSpider : MonoBehaviour {

    public Transform spider;
    public Transform threadAttachPoint;
    public Transform thread;

    private Rigidbody body;
    private ConfigurableJoint joint;

    // Use this for initialization
    void Start () {

        body = GetComponent<Rigidbody>();
        joint = GetComponent<ConfigurableJoint>();

        AttachJoint();
        joint.connectedAnchor = this.transform.position;

        UpdateThreadTransform();

        body.isKinematic = false;
    }

    void AttachJoint()
    {
        RaycastHit hit;

        // Check if a wall is near enough to stick
        if (Physics.Raycast(threadAttachPoint.position, Vector3.up, out hit, 10000))
        {
            //Save the position of the spider
            Vector3 spiderPosition = this.transform.position;

            //Move the anchor (the root gameobject) to the hit point;
            this.transform.position = hit.point;

            //Set the position of the spider
            spider.position = spiderPosition;
            spider.localPosition = new Vector3(0, 0, spider.localPosition.z);
        }
    }

    void UpdateThreadTransform()
    {
        // spider -> ceiling vector
        var v = transform.position - threadAttachPoint.position;

        thread.position = threadAttachPoint.position + v / 2f;

        thread.localScale = new Vector3(thread.localScale.x, v.magnitude / 2f, thread.localScale.z);

    }

    void Update()
    {
        // If the spider is destroyed, destroy everything
        if(spider == null)
        {
            Destroy(this.gameObject);
        }

       UpdateThreadTransform();
    }
}
