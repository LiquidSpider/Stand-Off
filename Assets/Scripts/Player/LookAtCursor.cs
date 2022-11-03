using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCursor : MonoBehaviour
{
    public GameObject cursor;

    void Update()
    {
        transform.LookAt(cursor.transform, Vector3.up);
    }
}
