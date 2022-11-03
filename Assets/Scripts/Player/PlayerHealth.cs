using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar] public float cHealth = 100f;
    public float maxHealth = 100f;

    public float headMul = 2f;
    public float bodyMul = 1f;
    public float limbMul = 0.5f;

    public float forceMul = 10;
    public float decapThreshold = 50;
    public float bodyLife = 10;
    float deathTimer = 0;

    public GameObject hitbox;

    GameManager gm;
    PlayerAnimations animations;
    NetworkManager manager;
    PlayerController pc;
    // Start is called before the first frame update
    void Start()
    {
        pc = GetComponent<PlayerController>();
        animations = transform.GetComponentInChildren<PlayerAnimations>();
        gm = FindObjectOfType<GameManager>();
        manager = FindObjectOfType<NetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(cHealth < 0)
        {
            Die(PlayerHitbox.HitBox.Body, 0, transform.position);
        }
        //if (gm.dead) {
        //    if (deathTimer > bodyLife) {
        //        Destroy(gameObject);
        //    }
        //    else {
        //        deathTimer += Time.deltaTime;
        //    }
        //}
    }

    public void Damage(PlayerHitbox.HitBox hitLocation, float damage, Vector3 hit, NetworkIdentity assailant) {

        switch (hitLocation) {
            case PlayerHitbox.HitBox.Head:
                cHealth -= damage * headMul;
                Debug.Log(gameObject.name + " was hit on " + hitLocation.ToString() + " for " + damage * headMul + " damage.");
                break;
            case PlayerHitbox.HitBox.Body:
                cHealth -= damage * bodyMul;
                Debug.Log(gameObject.name + " was hit on " + hitLocation.ToString() + " for " + damage * bodyMul + " damage.");
                break;
            case PlayerHitbox.HitBox.Limb:
                cHealth -= damage * limbMul;
                Debug.Log(gameObject.name + " was hit on " + hitLocation.ToString() + " for " + damage * limbMul + " damage.");
                break;
        }
        if (cHealth <= 0) {
            Die(hitLocation, damage, hit);
        }
        foreach (Rigidbody rb in transform.GetChild(0).GetComponentsInChildren<Rigidbody>()) {
            Vector3 direction = rb.position - hit;
            rb.AddForceAtPosition(direction.normalized * damage, hit, ForceMode.Impulse);
        }
    }

    public void Die(PlayerHitbox.HitBox hitLocation, float damage, Vector3 hit)
    {
        gm.dead = true;
        animations.Ragdoll(hit, forceMul);
        if (hitLocation == PlayerHitbox.HitBox.Head && Random.Range(0, 100) > decapThreshold)
        {
            animations.Decapitate();
        }
        hitbox.SetActive(false);
        //Destroy(animations.gameObject, bodyLife);
        CmdDie();
    }

    [Command]
    void CmdDie() {
        if (isLocalPlayer) {
            gm.dead = true;
            ClientScene.RemovePlayer();
        }
        RpcDie();
    }

    [ClientRpc]
    void RpcDie() {
        if (isLocalPlayer) {
            gm.dead = true;
            ClientScene.RemovePlayer();
        }
    }
}
