using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBackground : MonoBehaviour
{
    float hue = 0;
    float bright = 1f;
    public float colourSpeed = 0.5f;
    public bool darken = false;
    Camera camera;

    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (hue >= 1)
        {
            hue = 0;
        }
        hue += Time.deltaTime * colourSpeed;
        if (darken)
        {
            bright = Mathf.Lerp(bright, 0.4f, Time.deltaTime);
        }
        else
        {
            bright = Mathf.Lerp(bright, 1, Time.deltaTime);
        }
        camera.backgroundColor = Color.HSVToRGB(hue, 0.3f, bright);
    }
}
