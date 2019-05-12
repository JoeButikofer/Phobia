using UnityEngine;
using System.Collections;
using NewtonVR;

public enum ReactionType { NONE, FLEEING, STAND }

[RequireComponent(typeof(Animator))]
public class PhobicObjectController : MonoBehaviour {

    public Transform GroundPosition;
    private Vector3 target;
    public float speed;

    public ReactionType reaction;

    public bool stickToWall;

    [HideInInspector]
    public Rigidbody body;
    private Animator animator;

    private Vector3 planeNormal;
    private bool grounded;

    private bool scared;
    private bool isStanding;
    private bool caught;

    public void Catch(bool caught)
    {
        this.caught = caught;
    }

    // Use this for initialization
    void Start ()
    {
        if(body == null)
            body = GetComponent<Rigidbody>();

        animator = GetComponent<Animator>();
        planeNormal = transform.up;
    }

    void OnEnable()
    {
        StartCoroutine(ChooseDestination());
    }

    void OnDisable()
    {
        StopCoroutine(ChooseDestination());
    }

    // Update is called once per frame
    void Update ()
    {
        CheckGround();

        if (grounded)
        {
            isStanding = false;

            if (reaction != ReactionType.NONE && !caught)
                CheckReaction();

            if(!isStanding && !caught)
                Move();
            else
            {
                animator.SetFloat("speed", 0);
                animator.SetBool("IsMoving", false);
            }

            animator.SetBool("IsStanding", isStanding);
        }
    }

    void CheckGround()
    {
        var v = -transform.up * transform.localScale.x;

        RaycastHit hit;

        // Check if a wall is near enough to stick
        if (Physics.Raycast(transform.position, v, out hit, v.magnitude))
        {
            if (stickToWall && !caught)
            {
                Vector3 newPos = hit.point - (GroundPosition.position - transform.position);
                this.transform.position = newPos;

                body.useGravity = false;

                var targetRotation = Quaternion.LookRotation(transform.forward, hit.normal);

                planeNormal = transform.up;

                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10);
            }
            else
            {
                planeNormal = Vector3.up;
            }

            grounded = true;
        }
        else // Apply gravity
        {
            body.useGravity = true;

            planeNormal = Vector3.up;
            var targetRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10);
            grounded = false;
        }
    }

    void CheckReaction()
    {
        var hits = Physics.OverlapSphere(this.transform.position, 0.4f);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Hand"))
            {
                if (reaction == ReactionType.FLEEING)
                {
                    scared = true;
                    var v = this.transform.position - hit.transform.position;
                    target = this.transform.position + v;
                }
                else if (reaction == ReactionType.STAND)
                {
                    isStanding = true;

                    body.velocity = Vector3.zero;

                    // Face the hand
                    var v = hit.transform.position - this.transform.position;
                    v = Vector3.ProjectOnPlane(v, planeNormal);
                    transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(v.normalized, planeNormal), Time.deltaTime * 2);
                }
            }
        }
    }

    void Move()
    {
        var v = target - this.transform.position;
        v = Vector3.ProjectOnPlane(v, planeNormal);

        //if (v.magnitude > 0.1)
        {

            var actualSpeed = speed;
            if (scared)
                actualSpeed *= 2;

            animator.SetFloat("speed", actualSpeed / 5f);
            animator.SetBool("IsMoving", speed > 0.1f);

            body.velocity = v.normalized * actualSpeed * Time.deltaTime;

            transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(v.normalized, planeNormal), Time.deltaTime * speed / 5f);
        }
    }

    IEnumerator ChooseDestination()
    {
        while (true)
        {
            if (!scared)
            {
                var rand = Random.insideUnitSphere;
                target = this.transform.position + rand * 2;
            }
            else
                scared = false;

            yield return new WaitForSeconds(Random.Range(2f, 4f));
        }
    }
}
