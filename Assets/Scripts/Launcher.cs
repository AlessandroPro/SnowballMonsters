using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private byte maxPlayersPerRoom = 2;

    string gameVersion = "1";
    public Text connectionStatus;

    [Space(10)]
    [Header("Main Menu:")]
    public GameObject MainMenuUI;
    public Button joinLobbyButton;
    public InputField playerNameField;


    [Space(10)]
    [Header("Lobby:")]
    public GameObject LobbyUI;
    public InputField roomNameField;
    public Button createRoomButton;
    public Text welcomeText;
    public Text warningText;
    [SerializeField]
    private Transform roomListingContent;
    [SerializeField]
    private RoomListing roomListing;
    private List<RoomListing> listings = new List<RoomListing>();
    private HashSet<string> availableRoomNames = new HashSet<string>();

    [Space(10)]
    [Header("Room:")]
    public GameObject RoomUI;
    public Text playerStatus;
    public Button startGameButton;

    string playerName = "";
    string roomName = "";

    // Start Method
    void Start()
    {
        PlayerPrefs.DeleteAll();

        if(GameManager.returnToLobby)
        {
            playerName = PhotonNetwork.LocalPlayer.NickName;
            ShowLobby();
        }
        else
        {
            ShowMainMenu();
        }
    }

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
        PhotonNetwork.LocalPlayer.NickName = playerName;
    }

    public void SetRoomName(string name)
    {
        roomName = name;
    }

    public void ConnectToPhoton()
    {
        Debug.Log("Connecting to Photon Network");
        connectionStatus.text = "Connecting...";
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void JoinRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("PhotonNetwork.IsConnected! | Trying to Create/Join Room " + roomName);
            RoomOptions roomOptions = new RoomOptions();

            roomOptions.PublishUserId = true;
            roomOptions.MaxPlayers = 2;
            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);
            ShowRoom();
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby!");
    }

    // Photon Methods
    public override void OnConnectedToMaster()
    {

        base.OnConnected();

        connectionStatus.text = "Connected!";
        connectionStatus.color = Color.blue;

        PhotonNetwork.JoinLobby();
        ShowLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //isConnecting = false;
        Debug.LogError("Disconnected. Please check your Internet connection.");
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            playerStatus.text = "You are the Leader.";
        }
        else
        {
            playerStatus.text = "Waiting for the leader to start.";
            startGameButton.gameObject.SetActive(false);
        }
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
            int index = listings.FindIndex(x => x.roomInfo.Name == info.Name);
            if (index != -1)
            {
                // Removed from rooms list
                if (info.RemovedFromList)
                {
                    availableRoomNames.Remove(info.Name);
                    Destroy(listings[index].gameObject);
                    listings.RemoveAt(index);
                    
                }
                // Update Room List if full
                else if (info.PlayerCount == info.MaxPlayers)
                {

                    listings[index].joinButton.SetActive(false);
                }
                else
                {
                    listings[index].joinButton.SetActive(true);
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
                    availableRoomNames.Add(info.Name);
                }
            }
        }
    }


    // UI Changes

    private void ShowMainMenu()
    {
        MainMenuUI.SetActive(true);
        LobbyUI.SetActive(false);
        RoomUI.SetActive(false);
        joinLobbyButton.interactable = false;
        playerNameField.interactable = true;
    }

    private void ShowLobby()
    {
        MainMenuUI.SetActive(false);
        LobbyUI.SetActive(true);
        RoomUI.SetActive(false);

        welcomeText.text = "Welcome " + playerName + "!";
    }

    private void ShowRoom()
    {
        MainMenuUI.SetActive(false);
        LobbyUI.SetActive(false);
        RoomUI.SetActive(true);
    }
    // UI Actions

    public void OnClick_JoinLobby()
    {
        joinLobbyButton.interactable = false;
        playerNameField.interactable = false;
        ConnectToPhoton();
        SetPlayerName(playerNameField.text);
    }

    public void OnChange_PlayerNickname()
    {
        if(playerNameField.text == "" && joinLobbyButton.interactable)
        {
            joinLobbyButton.interactable = false;
        }
        else if(!joinLobbyButton.interactable)
        {
            joinLobbyButton.interactable = true;
        }
    }

    public void OnChange_NewRoomName()
    {
        if(availableRoomNames.Contains(roomNameField.text))
        {
            createRoomButton.interactable = false;
            warningText.gameObject.SetActive(true);
        }
        else
        {
            warningText.gameObject.SetActive(false);
            if(roomNameField.text != "")
            {
                createRoomButton.interactable = true;
            }
            else
            {
                createRoomButton.interactable = false;
            }
        }
    }

    public void OnClick_CreateRoom()
    {
        SetRoomName(roomNameField.text);
        JoinRoom();
    }

    public void OnClick_StartGame()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
        else
        {
            playerStatus.text = "2 Players required to Load Game!";
        }
    }
}
