using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerCustomization : NetworkBehaviour
{
    Renderer renderer;
    string playerName;
    Color color;
    public Text nameText;
    GameManager gm;
    public bool inMenu = false;
    // Start is called before the first frame update
    void Start()
    {
        renderer = transform.GetChild(0).GetComponentInChildren<Renderer>();
        color = new Color(PlayerPrefs.GetFloat("PlayerRed"), PlayerPrefs.GetFloat("PlayerGreen"), PlayerPrefs.GetFloat("PlayerBlue"));
        name = PlayerPrefs.GetString("Name", "Noob");
        gm = FindObjectOfType<GameManager>();
        if (!isLocalPlayer) {
            return;
        }
        if (inMenu) {
            ChangeColour(color);
            ChangeName(playerName);
        }
        else {
            CmdUpdatePlayer();
        }
    }

    public void ChangeColour(Color colour)
    {
        renderer.material.SetColor("_pColour", colour);
    }

    public void ChangeName(string name) {
        gameObject.name = name;
        nameText.text = name;
    }

    [Command]
    void CmdUpdatePlayer() {
        if (isLocalPlayer) {
            ChangeName(name);
            ChangeColour(color);
            RpcUpdatePlayer(name, color);
        }
    }

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        CmdUpdatePlayer();
    }

    [ClientRpc]
    public void RpcUpdatePlayer(string name, Color color) {
        if (!isLocalPlayer) return;
        ChangeColour(color);
        ChangeName(name);
    }
}
