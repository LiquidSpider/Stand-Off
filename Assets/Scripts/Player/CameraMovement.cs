using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float trackSpeed = 1;
    public bool track = true;

    private float camZ = -10;
    public GameObject Target;
    private Vector3 targetVector;

    void Start()
    {
        //SetTarget(GameObject.FindGameObjectWithTag("Player"));
    }

    void FixedUpdate()
    {
        if (track) Track();
    }

    void Track()
    {
        if (Target != null)
        {
            Vector3 vector = Vector3.Lerp(transform.position, Target.transform.position, Time.deltaTime * trackSpeed);
            vector.z = camZ;
            transform.position = vector;
        }
        else
        {
            Debug.LogError("No tracking target found.");
            track = false;
        }
    }

    public void SetTarget(GameObject target)
    {
        Target = target;
        Debug.Log("New camera target: " + target.name);
        track = true;
    }
}
