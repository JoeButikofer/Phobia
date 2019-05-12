using UnityEngine;
using System.Collections;

public class RandomMove : MonoBehaviour
{

    private Vector3 target;
    public float speed;

    private Vector3 planeNormal;

    void Start()
    {
        planeNormal = transform.up;
    }

    void OnEnable()
    {
        StartCoroutine(ChooseDestination());
    }

    // Update is called once per frame
    void Update()
    {
        var v = target - this.transform.position;
        if (v.magnitude > 0.1)
        {
            v = Vector3.ProjectOnPlane(v, planeNormal);

            this.transform.position += v.normalized * speed * Time.deltaTime;

            transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(v.normalized, planeNormal), Time.deltaTime * 2);
        }
    }

    IEnumerator ChooseDestination()
    {
        while (true)
        {
            var rand = Random.insideUnitSphere;
            target = this.transform.position + rand * 2;
            yield return new WaitForSeconds(Random.Range(2f,4f));
        }
    }
}
