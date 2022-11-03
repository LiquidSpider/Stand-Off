using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    //[SyncVar]
    public List<Player> players = new List<Player>();
    public List<uint> ids = new List<uint>();
    public float Gravity = 5;
    public float PlayerSpeed = 50;
    public float jumpHeight = 50;
    public bool logging = true;
    public bool isConnecting = false;
    public bool dead = false;
    public bool paused = false;

    public float shotVolume = 1;

    public int kills = 0;
    public int deaths = 0;

    public List<GameObject> Weapons = new List<GameObject>();

    NetworkManager manager;

    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity = new Vector3(0, -Gravity, 0);
        manager = GetComponent<NetworkManager>();
    }

    private void Update() {
        Connection();
        if (ClientScene.localPlayer == null) dead = true;
        else dead = false;
        if (!NetworkClient.isConnected) return;
        if (dead) {
            if (Input.GetButtonDown("Fire")) {
                //ClientScene.RemovePlayer();
                ClientScene.AddPlayer();
                dead = false;
            }
        }
    }

    public override void OnStartServer() {
        Player player = new Player(ClientScene.localPlayer.netId, PlayerPrefs.GetString("Name", "Noob"),
                        new Color(PlayerPrefs.GetFloat("PlayerRed", 1), PlayerPrefs.GetFloat("PlayerGreen", 1), PlayerPrefs.GetFloat("PlayerBlue", 1)));
        players.Add(player);
        ids.Add(player.netID);
        base.OnStartServer();
    }

    public override void OnStartClient() {
        
        base.OnStartClient();
    }

    void Connection() {
        if (!isConnecting) return;
        if (NetworkClient.isConnected && !ClientScene.ready) {
            ClientScene.Ready(NetworkClient.connection);
                if (ClientScene.localPlayer == null) {
                    ClientScene.AddPlayer();
                isConnecting = false;
            }
        }
    }

    public void BeginConnection() {
        manager.StartClient();
        isConnecting = true;
    }

    void UpdateColours() {
        foreach (Player player in players){
            foreach (NetworkIdentity identity in FindObjectsOfType<NetworkIdentity>()) {
                if (identity.netId == player.netID) {
                    identity.gameObject.GetComponent<PlayerCustomization>().ChangeColour(player.colour);
                    identity.gameObject.GetComponent<PlayerCustomization>().ChangeName(player.nickname);
                }
            }
        }
    }
}

public class Player{
    public uint netID;
    public string nickname;
    public Color colour;
    public int kills;
    public int deaths;
    //public GameObject weapon;

    public Player(uint nnetID, string nnickname, Color nColor) {
        netID = nnetID;
        nickname = nnickname;
        colour = nColor;
        kills = 0;
        deaths = 0;
    }
}
