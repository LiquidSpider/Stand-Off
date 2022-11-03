using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{

    /// <summary>
    /// PLAYER STUFF
    /// </summary>

    public GameObject lHand;
    public GameObject rHand;
    public GameObject equip;
    public GameObject currentWeapon;
    public GameObject cursor;
    public string playerName = "Player";
    public bool followMouse = true;
    public GameObject nameIndicator;
    Vector3 cursorPos;

    Rigidbody rb;
    GameManager gm;
    Animator animator;
    PlayerInventory Inventory;
    GameObject playerDetails;
    public GameObject gun;


    private Quaternion faceRight;
    private Quaternion faceLeft;

    private float moveSpeed;
    private float jumpHeight;
    private float walkModifier = 0.75f;
    private bool isGrounded;
    private float jumpDelay = 0.2f;
    private float jumpTime = 0.0f;
    private bool isFocussed = true;



    public enum Face
    {
        Left,
        Right
    };

    public Face facing = Face.Right;

    // Start is called before the first frame update
    void Start()
    {
        transform.gameObject.name = playerName;
        gm = FindObjectOfType<GameManager>();
        moveSpeed = gm.PlayerSpeed;
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        Inventory = GetComponentInChildren<PlayerInventory>();
        jumpHeight = gm.jumpHeight;
        if (GetComponentInChildren<GunScript>())
        {
            gun = GetComponentInChildren<GunScript>().gameObject;
        }
        if (isLocalPlayer) {
            Camera.main.GetComponent<CameraMovement>().SetTarget(gameObject);
        }

        ///
        /// GUN STUFF
        ///
        currentAcc = maxAcc;
        audio = GetComponentInChildren<AudioSource>();
        currentAmmo = magSize;
        gunRB = gun.GetComponent<Rigidbody>();
        gunRB.detectCollisions = false;
        gunRB.isKinematic = true;
        gunRB.detectCollisions = false;
    }

    private void OnApplicationFocus(bool focus)
    {
        isFocussed = focus;
    }

    void Update()
    {
        if (cursor.transform.position.x < transform.position.x)
            FaceDirection(Face.Left);
        else
            FaceDirection(Face.Right);
        PlayerInput();
        CheckGround();
        GetGun();
        if (!gm.paused) TrackCursor();
        if (Input.GetButtonUp("Fire") && hasFired) hasFired = false;
        fireTime += Time.deltaTime;
        currentAcc = Mathf.Lerp(currentAcc, maxAcc, incAcc);
    }

    void TrackCursor()
    {
        if (followMouse)
        {
            if (isFocussed && isLocalPlayer) {
                Ray castPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 vector = new Vector3(castPoint.origin.x, castPoint.origin.y, 0);
                cursor.transform.position = vector;
                if (isServer) RpcTrackCursor(vector);
            }
        }
    }

    [Command]
    void CmdTrackCursor(Vector3 position) {
        cursor.transform.position = position;
        RpcTrackCursor(position);
    }

    [ClientRpc]
    void RpcTrackCursor(Vector3 position) {
        cursor.transform.position = position;
    }

    private void LateUpdate()
    {
        transform.GetChild(0).localPosition = Vector3.zero;
        Vector3 zZero = new Vector3(transform.position.x, transform.position.y, 0);
        transform.position = zZero;
        CmdTrackCursor(cursor.transform.position);
    }

    void GetGun()
    {
        if (GetComponentInChildren<GunScript>())
        {
            gun = GetComponentInChildren<GunScript>().gameObject;
        }
        else
        {
            gun = null;
        }
    }
    void CheckGround()
    {
        isGrounded = Physics.CheckBox(new Vector3(transform.position.x, transform.position.y + 0.01827879f, transform.position.z), new Vector3(0.4258018f / 2, 0.0374603f, 0.4399817f), Quaternion.identity, LayerMask.GetMask("Terrain"));
        animator.SetBool("Grounded", isGrounded);
    }

    void PlayerInput()
    {
        if (!isLocalPlayer) return;
        if (gm.paused) return;
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W)) animator.ResetTrigger("Jump");
        if (gun)
        {
            if (Input.GetButton("Fire"))
            {
                Fire();
            }
            if (Input.GetButton("Reload"))
            {
                Reload();
            }
        }
        if (isFocussed)
        {
            if (Input.GetKey(KeyCode.D))
            {
                if (facing == Face.Right)
                {
                    rb.AddForce(Vector3.right * moveSpeed);
                    animator.SetFloat("Speed", 0.8f);
                }
                else
                {
                    rb.AddForce(Vector3.right * (moveSpeed * walkModifier));
                    animator.SetFloat("Speed", 0.2f);
                }
            }
            else
            {
                animator.SetFloat("Speed", 0);
            }
            if (Input.GetKey(KeyCode.A))
            {
                if (facing == Face.Left)
                {
                    rb.AddForce(Vector3.left * moveSpeed);
                    animator.SetFloat("Speed", 0.8f);
                }
                else
                {
                    rb.AddForce(Vector3.left * (moveSpeed * walkModifier));
                    animator.SetFloat("Speed", 0.2f);
                }
            }
            if (isGrounded)
            {
                jumpTime += Time.deltaTime;
                if (jumpTime > jumpDelay)
                {
                    if (jumpTime > 0.1f) {
                        animator.ResetTrigger("Jump");
                    }
                    if (Input.GetKeyDown(KeyCode.W))
                    {
                        //Debug.Log("Attempted to jump");
                        //rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                        CmdJump();
                        rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                        animator.SetTrigger("Jump");
                        jumpTime = 0;
                    }
                }
            }

        }

    }

    [Command]
    void CmdFace(Face face) {
        facing = face;
        FaceDirection(face);
        RpcFace(face);
    }

    [ClientRpc]
    void RpcFace(Face face) {
        facing = face;
        FaceDirection(face);
    }

    [Command]
    void CmdJump() {
        RpcJump(netId);
    }

    [ClientRpc]
    void RpcJump(uint sender) {
        if (isLocalPlayer) {
            return;
        }
        rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
    }

    void FaceDirection(Face face)
    {
        switch (face)
        {
            case Face.Left:
                facing = Face.Left;
                transform.rotation = Quaternion.Euler(0, 180, 0);
                nameIndicator.transform.localEulerAngles = new Vector3(0, 0, 0);
                //nameIndicator.transform.localPosition = new Vector3(0, nameIndicator.transform.localPosition.y, -50);
                break;
            case Face.Right:
                facing = Face.Right;
                transform.rotation = Quaternion.Euler(0, 0, 0);
                nameIndicator.transform.localEulerAngles = new Vector3(0, 180, 0);
                //nameIndicator.transform.localPosition = new Vector3(0, nameIndicator.transform.localPosition.y, 50);
                break;
        }
    }

    ///
    /// GUN STUFF
    ///

    public int currentAmmo = 0;
    public int magSize = 12;
    public float fireRate = 0.2f;
    public float tracerFadeSpeed = 10;
    public float damage = 25;
    public float maxAcc = 0;
    public float minAcc = 10;
    public float incAcc = 0.1f;
    public float decAcc = 0.5f;
    public enum FireModes { Semi, Full, Burst, Safe };
    public FireModes fireMode = FireModes.Semi;
    public enum HoldType { OneHand, TwoHand };
    public HoldType holdType = HoldType.OneHand;

    public int burstNum = 0;
    public AudioClip SFire;
    public float range = 20;
    public GameObject barrel;
    public bool isEquipped = false;


    string[] bulletLayers = new string[] { "Terrain", "Player", "Corpse" };
    bool isBursting = false;
    bool hasFired = false;
    float fireTime = 0;
    float currentAcc;
    AudioSource audio;
    Rigidbody gunRB;

    public void DeathCamera() {
        if (isLocalPlayer) {
            Camera.main.GetComponent<CameraMovement>().SetTarget(gameObject.transform.GetChild(0).gameObject);
            nameIndicator.SetActive(false);
        }
    }

    public void Drop() {
        gunRB.isKinematic = false;
        gunRB.detectCollisions = true;
        transform.SetParent(null, true);
    }

    public void Fire() {
        if (currentAmmo > 0) {
            if (fireTime > fireRate) {
                switch (fireMode) {
                    case FireModes.Semi:
                        if (!hasFired) {
                            if (isServer) {
                                HostShoot();
                            } else CmdShoot();
                            currentAcc = Mathf.Lerp(currentAcc, minAcc, decAcc);
                            //audio.PlayOneShot(SFire);
                            currentAmmo -= 1;
                            hasFired = true;
                        }
                        break;
                    case FireModes.Burst:
                        if (!hasFired) {
                            StartCoroutine(BurstFire());
                            hasFired = true;
                        }
                        break;
                    case FireModes.Full:
                        if (isServer) {
                            HostShoot();
                            }
                        else CmdShoot();
                        currentAcc = Mathf.Lerp(currentAcc, minAcc, decAcc);
                        //audio.PlayOneShot(SFire);
                        currentAmmo -= 1;
                        break;
                }
                fireTime = 0;
            }
        }
        else {
            Reload();
        }
    }

    public void Reload() {
        Debug.Log("Reloading");
        currentAmmo = magSize;
    }

    IEnumerator BurstFire() {
        if (!isBursting) {
            isBursting = true;
            for (int i = 0; i < burstNum; i++) {
                if (currentAmmo > 0) {
                    if (isServer) {
                        HostShoot();
                            }
                    else CmdShoot();
                    currentAcc = Mathf.Lerp(currentAcc, minAcc, decAcc);
                    //audio.PlayOneShot(SFire);
                    currentAmmo -= 1;
                    yield return new WaitForSeconds(fireRate);
                }
            }
            isBursting = false;
        }
    }

    public void DrawTracer(Vector3 start, Vector3 end, float life) {
        GameObject tracer = new GameObject();
        tracer.transform.parent = transform;
        //tracer.layer = 9;
        LineRenderer line = tracer.AddComponent<LineRenderer>();
        //NetworkIdentity identity = tracer.AddComponent<NetworkIdentity>();

        // Get line values from the renderer on the gun itself
        line.startWidth = GetComponent<LineRenderer>().startWidth;
        line.endWidth = GetComponent<LineRenderer>().endWidth;
        line.material = GetComponent<LineRenderer>().material;

        line.SetPosition(0, start);
        line.SetPosition(1, end);

        TracerFade fade = tracer.AddComponent<TracerFade>();
        fade.fadeSpeed = life;

    }

    //public GameObject CreateTracer(float life) {
    //    Debug.Log("Shoot!");
    //    Vector3 hitLocation;
    //    Vector3 direction;
    //    direction = barrel.transform.forward;
    //    //direction.x += Random.Range(-currentAcc, currentAcc);
    //    Quaternion newDirection = Quaternion.AngleAxis(Random.Range(-currentAcc, currentAcc), Vector3.right);
    //    direction = newDirection * direction;
    //    var layerMask = LayerMask.GetMask(bulletLayers);
    //    Ray shot = new Ray(barrel.transform.position, direction);
    //    if (Physics.Raycast(shot, out RaycastHit hit, range, layerMask, QueryTriggerInteraction.Collide)) {
    //        hitLocation = hit.point;

    //        if (hit.collider.gameObject.GetComponent<PlayerHitbox>()) {
    //            hit.collider.gameObject.GetComponent<PlayerHitbox>().Damage(damage, hitLocation);
    //        }
    //        Debug.Log("Hit something at X" + hitLocation.x + " Y" + hitLocation.y);
    //    }
    //    else {
    //        hitLocation = shot.GetPoint(range);
    //    }
    //    currentAcc = Mathf.Lerp(currentAcc, minAcc, decAcc);
    //    return null;
    //}

    void HostShoot() {
        Vector3 hitLocation;
        Vector3 direction;
        direction = barrel.transform.forward;
        //direction.x += Random.Range(-currentAcc, currentAcc);
        Quaternion newDirection = Quaternion.AngleAxis(Random.Range(-currentAcc, currentAcc), Vector3.right);
        direction = newDirection * direction;
        var layerMask = LayerMask.GetMask(bulletLayers);
        Ray shot = new Ray(barrel.transform.position, direction);
        Debug.Log("Player " + ClientScene.localPlayer.gameObject.name + " fired");
        if (Physics.Raycast(shot, out RaycastHit hit, range, layerMask, QueryTriggerInteraction.Collide)) {
            hitLocation = hit.point;

            if (hit.collider.gameObject.GetComponent<PlayerHitbox>()) {
                hit.collider.gameObject.GetComponent<PlayerHitbox>().Damage(damage, hitLocation, ClientScene.localPlayer);
            }
            Debug.Log("Hit something at X" + hitLocation.x + " Y" + hitLocation.y);
        }
        else {
            hitLocation = shot.GetPoint(range);
        }
        Debug.Log("Server tried to draw line from " + shot.origin.ToString() + " to " + hitLocation.ToString());
        RpcRenderTracer(shot.origin, hitLocation, tracerFadeSpeed);
    }

    [Command]
    void CmdShoot() {
        Vector3 hitLocation;
        Vector3 direction;
        direction = barrel.transform.forward;
        //direction.x += Random.Range(-currentAcc, currentAcc);
        Quaternion newDirection = Quaternion.AngleAxis(Random.Range(-currentAcc, currentAcc), Vector3.right);
        direction = newDirection * direction;
        var layerMask = LayerMask.GetMask(bulletLayers);
        Ray shot = new Ray(barrel.transform.position, direction);
        Debug.Log("Player " + ClientScene.localPlayer.gameObject.name + " fired");
        if (Physics.Raycast(shot, out RaycastHit hit, range, layerMask, QueryTriggerInteraction.Collide)) {
            hitLocation = hit.point;

            if (hit.collider.gameObject.GetComponent<PlayerHitbox>()) {
                hit.collider.gameObject.GetComponent<PlayerHitbox>().Damage(damage, hitLocation, ClientScene.localPlayer);
            }
            Debug.Log("Hit something at X" + hitLocation.x + " Y" + hitLocation.y);
        }
        else {
            hitLocation = shot.GetPoint(range);
        }
        //Debug.Log("Told server to draw line from " + shot.origin.ToString() + " to " + hitLocation.ToString());
        RpcRenderTracer(shot.origin, hitLocation, tracerFadeSpeed);
    }

    [ClientRpc]
    void RpcRenderTracer(Vector3 start, Vector3 end, float life) {
        DrawTracer(start, end, life);
        audio.PlayOneShot(SFire);
    }
}
