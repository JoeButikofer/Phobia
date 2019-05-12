using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;


public delegate void OnNoteTouched(Note note);

public class NoteMouseEvent : MonoBehaviour
    , IPointerClickHandler
    , IPointerEnterHandler
{
    public Note note;
    public event OnNoteTouched onNoteTouched;

    // Use this for initialization
    void Start ()
    {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnPointerClick(PointerEventData eventData)
    {
        onNoteTouched(note);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }
}
