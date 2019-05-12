using UnityEngine;
using System.Collections;

public class DestroyPhobicOnEnter : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PhobicObject"))
        {
            Destroy(other.gameObject);
        }
    }
}
