using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    public bool isTracking = true;
    public bool ragdolled = false;
    public bool preview = false;
    public enum MainHand
    {
        Right,
        Left
    }
    public MainHand handedness = MainHand.Right;

    public GameObject head;

    public GameObject cursor;
    GunScript gun;
    public Animator animator;
    GunScript.HoldType holdType = GunScript.HoldType.OneHand;
    PlayerController pc;
    Rigidbody mainBody;

    // Start is called before the first frame update
    void Start()
    {
        mainBody = transform.root.GetComponent<Rigidbody>();
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.detectCollisions = false;
            rb.mass = 3;
            rb.drag = 5;
        }
        pc = transform.root.GetComponent<PlayerController>();
        //animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ragdolled)
        {
            Ragdoll();
        }
        if (GetComponentInChildren<GunScript>())
        {
            holdType = GetComponentInChildren<GunScript>().holdType;
        }
        gun = GetComponentInChildren<GunScript>();
        if (preview) animator.SetFloat("Speed", 2f);
    }

    void Ragdoll()
    {
        animator.enabled = false;
        //mainBody.constraints = RigidbodyConstraints.None;
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
            if (rb.gameObject.name == "Hips")
            {
                rb.velocity = mainBody.velocity;
            }

        }
        GameObject body = new GameObject();
        transform.parent = body.transform;
        Destroy(body, 10);
        //Camera.main.GetComponent<CameraMovement>().SetTarget(body);
        pc.enabled = false;
    }

    public void Ragdoll(Vector3 hit, float force)
    {
        animator.enabled = false;
        //mainBody.constraints = RigidbodyConstraints.None;
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
            rb.velocity = mainBody.velocity;
            Vector3 direction = rb.position - hit;
            //rb.AddForceAtPosition(direction * force, hit, ForceMode.Impulse);
        }
        //transform.parent.gameObject.layer = LayerMask.NameToLayer("Corpse");
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = LayerMask.NameToLayer("Corpse");
        }
        GameObject body = new GameObject();
        transform.parent = body.transform;
        Destroy(body, 10);
        //Camera.main.GetComponent<CameraMovement>().SetTarget(body.transform.GetChild(0).gameObject);
        pc.enabled = false;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (isTracking)
        {
            Vector3 handRot = transform.root.GetComponentInChildren<LookAtCursor>().transform.rotation.eulerAngles;
            handRot.z -= 90;
            Transform LookGoal = cursor.transform;
            Transform IKGoal = cursor.transform.GetChild(0).transform;
            animator.SetLookAtPosition(LookGoal.position);
            animator.SetLookAtWeight(1, 0.1f, 0.5f, 0, 0.5f);

            if (pc.currentWeapon == null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            }
            else
            {
                if (holdType == GunScript.HoldType.OneHand)
                {
                    if (handedness == MainHand.Right)
                    {
                        // Right Hand
                        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                        animator.SetIKPosition(AvatarIKGoal.RightHand, IKGoal.position);
                        animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.Euler(handRot));
                    }
                    else
                    {
                        // Left Hand
                        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                        animator.SetIKPosition(AvatarIKGoal.LeftHand, IKGoal.position);
                        animator.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.Euler(handRot));
                    }

                }
                else if (holdType == GunScript.HoldType.TwoHand)
                {
                    // Right Hand
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, IKGoal.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.Euler(handRot));

                    // Left Hand
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, gun.transform.parent.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.Euler(handRot));
                }
                else
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                }
            }
        }
        else
        {
            animator.SetLookAtWeight(0);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
        }
        
    }

    public void Flinch(PlayerHitbox.HitBox hit, bool direction)
    {
        switch (hit)
        {
            case PlayerHitbox.HitBox.Head:
                switch (direction)
                {
                    case true:
                        if (pc.facing == PlayerController.Face.Right)
                            animator.SetTrigger("HeadFront");
                        if (pc.facing == PlayerController.Face.Left)
                            animator.SetTrigger("HeadBack");
                        break;
                    case false:
                        if (pc.facing == PlayerController.Face.Right)
                            animator.SetTrigger("HeadBack");
                        if (pc.facing == PlayerController.Face.Left)
                            animator.SetTrigger("HeadFront");
                        break;
                }
                break;
            case PlayerHitbox.HitBox.Body:
                switch (direction)
                {
                    case true:
                        if (pc.facing == PlayerController.Face.Right)
                            animator.SetTrigger("BodyFront");
                        if (pc.facing == PlayerController.Face.Left)
                            animator.SetTrigger("BodyBack");
                        break;
                    case false:
                        if (pc.facing == PlayerController.Face.Right)
                            animator.SetTrigger("BodyBack");
                        if (pc.facing == PlayerController.Face.Left)
                            animator.SetTrigger("BodyFront");
                        break;
                }
                break;
        }
    }

    public void Decapitate()
    {
        head.GetComponent<SphereCollider>().enabled = false;
        head.transform.parent.localScale = Vector3.zero;
        head.transform.localPosition = Vector3.zero;
    }

}
