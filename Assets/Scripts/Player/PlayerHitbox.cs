using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerHitbox : MonoBehaviour
{
    public enum HitBox { Head, Body, Limb};
    public HitBox hitBox = HitBox.Body;

    PlayerHealth health;
    PlayerAnimations animations;
    // Start is called before the first frame update
    void Start()
    {
        health = transform.root.GetComponent<PlayerHealth>();
        animations = transform.root.GetComponentInChildren<PlayerAnimations>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(float damage, Vector3 hit, NetworkIdentity assailant)
    {
        health.Damage(hitBox, damage, hit, assailant);
        bool direction = true; // True = hit from the right, false = hit from the left
        if (hit.x > transform.position.x) direction = true;
        if (hit.x < transform.position.x) direction = false;
        animations.Flinch(hitBox, direction);
    }
}
