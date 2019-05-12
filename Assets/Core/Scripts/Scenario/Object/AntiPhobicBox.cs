using UnityEngine;
using System.Collections;

public class AntiPhobicBox : MonoBehaviour
{
    public bool IsReusable;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PhobicObject"))
        {
            animator.SetBool("IsOpen", false);
            StartCoroutine(WaitAndDestroy(other.gameObject));
        }
    }

    IEnumerator WaitAndDestroy(GameObject phobicObject)
    {
        // Wait for the end of the animation
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);

        Destroy(phobicObject);

        if(IsReusable)
        {
            yield return new WaitForSeconds(2f);
            animator.SetBool("IsOpen", true);
        }
    }
}
