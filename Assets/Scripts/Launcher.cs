using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject controlPanel;

    [SerializeField]
    private Text feedbackText;

    [SerializeField]
    private byte maxPlayersPerRoom = 2;

    bool isConnecting;

    string gameVersion = "1";

    [Space(10)]
    [Header("Custom Variables")]
    public InputField playerNameField;
    public InputField roomNameField;

    [Space(5)]
    public Text playerStatus;
    public Text connectionStatus;

    [Space(5)]
    public GameObject roomJoinUI;
    public GameObject buttonLoadArena;
    public GameObject buttonJoinRoom;

    string playerName = "";
    string roomName = "";

    // Room listings
    [SerializeField]
    private Transform roomListingContent;

    [SerializeField]
    private RoomListing roomListing;

    private List<RoomListing> listings = new List<RoomListing>();


    // Start Method
    void Start()
    {
        //1
        PlayerPrefs.DeleteAll();

        Debug.Log("Connecting to Photon Network");

        //2
        roomJoinUI.SetActive(false);
        buttonLoadArena.SetActive(false);

        //3
        ConnectToPhoton();
    }

    void Awake()
    {
        //4 
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Helper Methods
    public void SetPlayerName(string name)
    {
        playerName = name;
        PhotonNetwork.LocalPlayer.NickName = playerName;
    }

    public void SetRoomName(string name)
    {
        roomName = name;
    }

    public void SetRoomNameText(string name)
    {
        roomNameField.text = name;
        SetRoomName(name);
    }

    void ConnectToPhoton()
    {
        connectionStatus.text = "Connecting...";
        PhotonNetwork.GameVersion = gameVersion; //1
        PhotonNetwork.ConnectUsingSettings(); //2
    }

    public void JoinRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            SetRoomName(roomNameField.text);
            SetPlayerName(playerNameField.text);

            Debug.Log("PhotonNetwork.IsConnected! | Trying to Create/Join Room " + roomName);
            RoomOptions roomOptions = new RoomOptions(); //2
            //TypedLobby typedLobby = new TypedLobby(roomName, LobbyType.Default); //3
            //PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, typedLobby); //4

            roomOptions.PublishUserId = true;
            roomOptions.MaxPlayers = 2;
            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("JOINED LOBBY!!");
    }

    public void LoadArena()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 0)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
        else
        {
            playerStatus.text = "Minimum 2 Players required to Load Arena!";
        }
    }

    // Photon Methods
    public override void OnConnectedToMaster()
    {
        // 1
        base.OnConnected();
        // 2
        connectionStatus.text = "Connected to Photon!";
        connectionStatus.color = Color.green;
        roomJoinUI.SetActive(true);
        buttonLoadArena.SetActive(false);

        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        // 3
        isConnecting = false;
        controlPanel.SetActive(true);
        Debug.LogError("Disconnected. Please check your Internet connection.");
    }

    public override void OnJoinedRoom()
    {
        // 4
        if (PhotonNetwork.IsMasterClient)
        {
            buttonLoadArena.SetActive(true);
            buttonJoinRoom.SetActive(false);
            playerStatus.text = "Your are Lobby Leader";
        }
        else
        {
            playerStatus.text = "Connected to Lobby";
        }

        //LoadArena();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created room successfully!");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room creation failed: " + message);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Rooms List Updated!");
        foreach(RoomInfo info in roomList)
        {
            // Removed from rooms list
            if(info.RemovedFromList)
            {
                int index = listings.FindIndex(x => x.RoomInfo.Name == info.Name);
                if(index != -1)
                {
                    Destroy(listings[index].gameObject);
                    listings.RemoveAt(index);
                }
            }
            // Added to rooms list
            else
            {
                RoomListing listing = Instantiate(roomListing, roomListingContent);
                if (listing != null)
                {
                    listing.SetRoomInfo(info);
                    listings.Add(listing);
                }
            }
        }
    }
}
