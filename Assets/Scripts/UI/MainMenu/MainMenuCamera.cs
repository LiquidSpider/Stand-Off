using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class MainMenuCamera : MonoBehaviour
{
    public GameObject main;
    public GameObject join;
    public InputField ip;
    public InputField port;
    public Text connectionLog;
    NetworkManager manager;
    GameManager gm;

    Camera camera;
    CameraMovement cameraMovement;

    void Start()
    {
        manager = FindObjectOfType<NetworkManager>();
        gm = FindObjectOfType<GameManager>();
        camera = Camera.main;
        cameraMovement = camera.GetComponent<CameraMovement>();
        cameraMovement.SetTarget(main);
        join.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeMenu(GameObject menu)
    {
        cameraMovement.SetTarget(menu);
        if (menu.name == "CharacterCustomization")
        {
            FindObjectOfType<MainMenuBackground>().darken = true;
        }
        else
        {
            FindObjectOfType<MainMenuBackground>().darken = false;
        }
    }

    public void OpenJoin() {
        if (!join.activeInHierarchy) join.SetActive(true);
    }

    public void CloseJoin() {
        if (join.activeInHierarchy) join.SetActive(false);
    }

    public void JoinGame() {
        connectionLog.text = "Connecting to " + manager.networkAddress + " on port " + port.text + "...";
        gm.BeginConnection();
    }

    public void HostGame() {
        manager.StartHost();
    }

    public void CloseGame() {
        Application.Quit();
    }

    public void ChangeIP() {
        manager.networkAddress = ip.text;
        manager.GetComponent<TelepathyTransport>().port = ushort.Parse(port.text);
    }
}
