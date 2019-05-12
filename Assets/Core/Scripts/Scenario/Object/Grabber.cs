using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewtonVR;
using UnityEngine;

class Grabber : MonoBehaviour
{

    public float MaxLength;
    public float MinLength;
    public float GrabRadius;

    public NVRHand Hand;
    public Transform GrabTransform;

    private float step = 1;
    private float length;
    private Transform grabbed;

    void Start()
    {
        length = MinLength;
        UpdateTransform();
    }

    void Update()
    {
        if (Hand != null)
        {
            transform.position = Hand.transform.position;
            transform.rotation = Hand.transform.rotation;

            if (Hand.Inputs[NVRButtons.Touchpad].IsPressed)
            {
                var touchpad = Hand.Inputs[NVRButtons.Touchpad].Axis;

                if (touchpad.y > 0.6f)
                {
                    //Moving up
                    ChangeLength(step);
                }

                else if (touchpad.y < -0.6f)
                {
                    //Moving down
                    ChangeLength(-step);
                }
            }
            GrabTransform.position = transform.position + transform.forward * length * 2;

            if(grabbed != null)
            {
                grabbed.localPosition = Vector3.zero;
                grabbed.GetComponent<Rigidbody>().velocity = Vector3.zero;

                if (Hand.Inputs[NVRButtons.Trigger].PressUp)
                {
                    //Release the grabbed object
                    Release();
                }
             }

            if (Hand.Inputs[NVRButtons.Trigger].PressDown)
            {
                //Grab the closest object
                PickupClosest();
            }
        }
    }

    void ChangeLength(float delta)
    {
        float tmpLength = length + delta * Time.unscaledDeltaTime;

        if(tmpLength > MaxLength)
        {
            length = MaxLength;
        }
        else if (tmpLength < MinLength)
        {
            length = MinLength;
        }
        else
        {
            length = tmpLength;
        }

        UpdateTransform();
    }

    void UpdateTransform()
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, length);
    }

    private void PickupClosest()
    {
        NVRInteractable closest = null;
        float closestDistance = float.MaxValue;

        var hits = Physics.OverlapSphere(GrabTransform.position, 0.4f);

        foreach (var hit in hits)
        {
            var interactable = hit.transform.GetComponent<NVRInteractable>();
            if (interactable == null)
                continue;

            float distance = Vector3.Distance(this.transform.position, hit.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = interactable;
            }
        }

        if (closest != null)
        {
            Grab(closest);
        }
    }

    private void Grab(NVRInteractable obj)
    {
        if (obj is NVRInteractableItem)
        {
            NVRInteractableItem item = obj as NVRInteractableItem;
            item.OnBeginInteraction.Invoke();
        }

        obj.transform.SetParent(GrabTransform);
        obj.transform.localPosition = Vector3.zero;
        obj.Rigidbody.velocity = Vector3.zero;

        grabbed = obj.transform;
    }

    void Release()
    {
        var item = grabbed.GetComponent<NVRInteractableItem>();
        if(item != null)
        {
            item.OnEndInteraction.Invoke();
        }

        grabbed.SetParent(null);
        grabbed = null;
    }
}

