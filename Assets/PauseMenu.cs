using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;

public class PauseMenu : MonoBehaviour
{
    public bool inGame = false;
    bool pauseOpen = false;
    public Canvas pauseMenu;
    GameManager gm;
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        if (!pauseOpen) {
            pauseMenu.enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!inGame) return;
            if (pauseOpen) {
                pauseOpen = false;
                pauseMenu.enabled = false;
                gm.paused = false;
            }
            else {
                pauseOpen = true;
                pauseMenu.enabled = true;
                gm.paused = true;
            }
        }
        if (pauseMenu.worldCamera == null) {
            pauseMenu.worldCamera = Camera.main;
        }
        if (SceneManager.GetActiveScene().name == "MainMenu") {
            FindObjectOfType<NetworkManagerHUD>().showGUI = true;
            inGame = false;
        }
        else {
            inGame = true;
            FindObjectOfType<NetworkManagerHUD>().showGUI = false;
        }
    }

    public void Disconnect() {
        Debug.Log("Disconnecting...");
        FindObjectOfType<NetworkManagerHUD>().showGUI = true;
        FindObjectOfType<NetworkManager>().StopHost();
    }

    public void Quit() {
        Debug.Log("Quitting...");
        FindObjectOfType<NetworkManagerHUD>().showGUI = true;
        Application.Quit();
    }
}
