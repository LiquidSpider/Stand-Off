using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    public Slider SFX;
    public AudioMixerGroup SFXGroup;

    GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        SFX.value = PlayerPrefs.GetFloat("SFXVolume", 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeVolume() {
        //gm.shotVolume = Mathf.Log10(SFX.value) * 20;
        SFXGroup.audioMixer.SetFloat("SFXVol", Mathf.Log10(SFX.value) * 20);
        PlayerPrefs.SetFloat("SFXVolume", SFX.value);
    }
}
