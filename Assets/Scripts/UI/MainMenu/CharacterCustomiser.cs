using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCustomiser : MonoBehaviour
{
    public GameObject player;
    public Slider red;
    public Slider green;
    public Slider blue;
    public InputField name;
    public Text nameText;
    //bool init = false;
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("PlayerRed")) PlayerPrefs.SetFloat("PlayerRed", 1);
        if (!PlayerPrefs.HasKey("PlayerGreen")) PlayerPrefs.SetFloat("PlayerGreen", 1);
        if (!PlayerPrefs.HasKey("PlayerBlue")) PlayerPrefs.SetFloat("PlayerBlue", 1);
        if (!PlayerPrefs.HasKey("Name")) PlayerPrefs.SetString("Name", "Player");
        red.value = PlayerPrefs.GetFloat("PlayerRed");
        green.value = PlayerPrefs.GetFloat("PlayerGreen");
        blue.value = PlayerPrefs.GetFloat("PlayerBlue");
        name.text = PlayerPrefs.GetString("Name");
        UpdateColours();
        ChangeName();
    }
    private void Update()
    {
        UpdateColours();
    }
    public void UpdateColours()
    {
        PlayerPrefs.SetFloat("PlayerRed", red.value);
        PlayerPrefs.SetFloat("PlayerGreen", green.value);
        PlayerPrefs.SetFloat("PlayerBlue", blue.value);
        ChangeColour();
    }

    void ChangeColour()
    {
        Color color = new Color(PlayerPrefs.GetFloat("PlayerRed"),
                                PlayerPrefs.GetFloat("PlayerGreen"),
                                PlayerPrefs.GetFloat("PlayerBlue"));
        player.GetComponent<PlayerCustomization>().ChangeColour(color);
    }

    public void ChangeName() {
        PlayerPrefs.SetString("Name", name.text);
        nameText.text = name.text;
    }
}
