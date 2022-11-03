using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    public Color dummyColor;
    public bool randomColor;

    Renderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        renderer = transform.GetChild(0).GetComponentInChildren<Renderer>();
        if (randomColor)
        {
            dummyColor = new Color(Random.value, Random.value, Random.value);
        }
        renderer.material.SetColor("_pColour", dummyColor);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
