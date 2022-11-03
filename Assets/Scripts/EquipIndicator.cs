using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipIndicator : MonoBehaviour
{
    public GameObject WeaponText;
    public GameObject AmmoText;

    PlayerController pc;
    GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        WeaponText.GetComponent<Text>().text = "";
    }
}
