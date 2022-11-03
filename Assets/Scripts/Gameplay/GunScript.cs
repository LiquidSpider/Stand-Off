using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public int currentAmmo = 0;
    public int magSize = 12;
    public float fireRate = 0.2f;
    public float tracerFadeSpeed = 10;
    public float damage = 25;
    public float maxAcc = 0;
    public float minAcc = 10;
    public float incAcc = 0.1f;
    public float decAcc = 0.5f;
    public enum FireModes { Semi, Full, Burst, Safe};
    public FireModes fireMode = FireModes.Semi;
    public enum HoldType { OneHand, TwoHand };
    public HoldType holdType = HoldType.OneHand;

    public int burstNum = 0;
    public AudioClip SFire;
    public float range = 20;
    public GameObject barrel;
    public bool isEquipped = false;


    string[] bulletLayers = new string[] { "Terrain", "Player", "Corpse"};
    bool isBursting = false;
    bool hasFired = false;
    float fireTime = 0;
    float currentAcc;
    AudioSource audio;
    Rigidbody rb;
    PlayerController pc;

    // Start is called before the first frame update
    void Start()
    {
        currentAcc = maxAcc;
        audio = GetComponentInChildren<AudioSource>();
        currentAmmo = magSize;
        GetComponent<Rigidbody>().detectCollisions = false;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.detectCollisions = false;
        pc = transform.root.GetComponent<PlayerController>();
    }

    void Update()
    {
        
        if (Input.GetButtonUp("Fire") && hasFired) hasFired = false;
        fireTime += Time.deltaTime;
        currentAcc = Mathf.Lerp(currentAcc, maxAcc, incAcc);
    }

    public void Drop()
    {
        rb.isKinematic = false;
        rb.detectCollisions = true;
        transform.SetParent(null, true);
    }

    public void Fire()
    {
        if (currentAmmo > 0)
        {
            if (fireTime > fireRate)
            {
                switch (fireMode)
                {
                    case FireModes.Semi:
                        if (!hasFired)
                        {
                            CreateTracer(tracerFadeSpeed);
                            audio.PlayOneShot(SFire);
                            currentAmmo -= 1;
                            hasFired = true;
                        }
                        break;
                    case FireModes.Burst:
                        if (!hasFired)
                        {
                            StartCoroutine(BurstFire());
                            hasFired = true;
                        }
                        break;
                    case FireModes.Full:
                        CreateTracer(tracerFadeSpeed);
                        audio.PlayOneShot(SFire);
                        currentAmmo -= 1;
                        break;
                }
                fireTime = 0;
            }
        }
        else
        {
            Reload();
        }
    }

    public void Reload()
    {
        Debug.Log("Reloading");
        currentAmmo = magSize;
    }

    IEnumerator BurstFire()
    {
        if (!isBursting)
        {
            isBursting = true;
            for (int i = 0; i < burstNum; i++)
            {
                if (currentAmmo > 0)
                {
                    CreateTracer(tracerFadeSpeed);
                    audio.PlayOneShot(SFire);
                    currentAmmo -= 1;
                    yield return new WaitForSeconds(fireRate);
                }
            }
            isBursting = false;
        }
    }

    public GameObject CreateTracer(float life)
    {
        Debug.Log("Shoot!");
        Vector3 hitLocation;
        Vector3 direction;
        direction = barrel.transform.forward;
        //direction.x += Random.Range(-currentAcc, currentAcc);
        Quaternion newDirection = Quaternion.AngleAxis(Random.Range(-currentAcc, currentAcc), Vector3.right);
        direction = newDirection * direction;
        var layerMask = LayerMask.GetMask(bulletLayers);
        Ray shot = new Ray(barrel.transform.position, direction);
        if (Physics.Raycast(shot, out RaycastHit hit, range, layerMask, QueryTriggerInteraction.Collide))
        {
            hitLocation = hit.point;
            
            if (hit.collider.gameObject.GetComponent<PlayerHitbox>())
            {
                hit.collider.gameObject.GetComponent<PlayerHitbox>().Damage(damage, hitLocation, null);
            }
            Debug.Log("Hit something at X" + hitLocation.x + " Y" + hitLocation.y);
        }
        else
        {
            hitLocation = shot.GetPoint(range);
        }
        GameObject tracer = new GameObject();
        tracer.transform.parent = transform;
        //tracer.layer = 9;
        LineRenderer line = tracer.AddComponent<LineRenderer>();
        //NetworkIdentity identity = tracer.AddComponent<NetworkIdentity>();

        // Get line values from the renderer on the gun itself
        line.startWidth = GetComponent<LineRenderer>().startWidth;
        line.endWidth = GetComponent<LineRenderer>().endWidth;
        line.material = GetComponent<LineRenderer>().material;

        line.SetPosition(0, barrel.transform.position);
        line.SetPosition(1, hitLocation);

        TracerFade fade = tracer.AddComponent<TracerFade>();
        fade.fadeSpeed = life;
        //FindObjectOfType<GameManager>().Shoot(tracer);

        currentAcc = Mathf.Lerp(currentAcc, minAcc, decAcc);
        return tracer;
    }

    public void Unequip()
    {
        Destroy(gameObject);
    }
}
