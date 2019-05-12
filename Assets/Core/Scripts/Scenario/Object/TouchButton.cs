using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

class TouchButton : OnTouch
{
    public UnityEvent buttonTouched;

    void Awake()
    {
        if (buttonTouched == null)
            buttonTouched = new UnityEvent();
    }

    protected override void Touch()
    {
        buttonTouched.Invoke();
    }
}
