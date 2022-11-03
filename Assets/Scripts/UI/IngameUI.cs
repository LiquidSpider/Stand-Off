using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

public class IngameUI : NetworkBehaviour
{
    public GameObject UI;
    public Slider health;
    public Text healthText;
    public Text respawn;
    public bool UIActive = false;
    GameObject player;
    GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu") {
            UI.SetActive(false);
            UIActive = false;
        }
        else {
            UI.SetActive(true);
            UIActive = true;
        }
        ShowHealth();
    }

    void ShowHealth() {
        if (!UIActive) return;
        if (ClientScene.localPlayer) {
            player = ClientScene.localPlayer.gameObject;
        }
        else {
            player = null;
        }
        if (player) {
            if (player.GetComponent<PlayerHealth>()) {
                health.value = player.GetComponent<PlayerHealth>().cHealth;
                healthText.text = health.value.ToString("F0");
            }
        }
        if (gm.dead || ClientScene.localPlayer == null) {
            healthText.text = "DEAD";
            respawn.enabled = true;
        }
        else {
            respawn.enabled = false;
        }
    }
}
