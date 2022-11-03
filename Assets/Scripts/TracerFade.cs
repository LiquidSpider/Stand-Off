using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracerFade : MonoBehaviour
{

    public float fadeSpeed = 1f;

    LineRenderer line;

    private void Start()
    {
        if (GetComponent<LineRenderer>())
        {
            line = GetComponent<LineRenderer>();
        }
        else
        {
            Debug.LogError("TracerFade attempted to fade a line that doesn't exist");
        }    
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (line != null)
        {
            if (line.startColor.a > 0)
            {
                Color newAlpha = new Color(line.startColor.r, line.startColor.g, line.startColor.b, line.startColor.a - (Time.deltaTime * fadeSpeed));
                line.startColor = newAlpha;
                line.endColor = newAlpha;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
