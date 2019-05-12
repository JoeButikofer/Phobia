using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum Member { HAND, FOOT, EITHER}

[RequireComponent (typeof(Collider))] // The collider MUST be a trigger (isTrigger == true)
public abstract class OnTouch : MonoBehaviour
{
    public Member allowedMember;

    void OnTriggerEnter(Collider other)
    {
        if (allowedMember == Member.HAND || allowedMember == Member.EITHER)
        {
            if (other.tag == "Hand")
            {
                Touch();
            }
        }
        else if (allowedMember == Member.FOOT || allowedMember == Member.EITHER)
        {
            if (other.tag == "Foot")
            {
                Touch();
            }
        }
    }

    protected abstract void Touch();
}
