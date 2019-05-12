using UnityEngine;
using System.Collections;

public class IKHand : MonoBehaviour {

    [SerializeField]
    private Transform leftHandTransform;

    [SerializeField]
    private Transform rightHandTransform;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK()
    {
        if (animator)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);

            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTransform.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTransform.rotation);

            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);

            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTransform.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTransform.rotation);
        }
    }
}
