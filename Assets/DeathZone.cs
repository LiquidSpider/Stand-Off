using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{

    
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.transform.root.GetComponent<PlayerHealth>()) {
            other.gameObject.transform.root.GetComponent<PlayerHealth>().cHealth = 0;
            other.gameObject.transform.root.GetComponent<PlayerHealth>().Die(PlayerHitbox.HitBox.Body, 0, other.transform.position);
        }
    }
}
