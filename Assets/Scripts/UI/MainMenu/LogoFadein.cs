using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogoFadein : MonoBehaviour
{
    Color alpha;
    bool fading = true;
    public float fadeSpeed = 1;

    void Start()
    {
        alpha = GetComponent<Image>().color;
        alpha.a = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (fading) alpha.a += Time.deltaTime * fadeSpeed;
        GetComponent<Image>().color = alpha;
        if (alpha.a >= 1) fading = false;
    }
}
