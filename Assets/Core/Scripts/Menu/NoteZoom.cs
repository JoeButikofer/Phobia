using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class NoteZoom : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public float ZoomFactor = 2f;

    private Vector3 baseScale;

    // Use this for initialization
    void Start()
    {
        baseScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = baseScale * ZoomFactor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = baseScale;
    }
}
