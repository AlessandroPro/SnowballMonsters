using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour
{

    [SerializeField]
    private Text _text;
    private Launcher launcher;
    public GameObject joinButton;

    public RoomInfo roomInfo { get; private set; }

    private void Start()
    {
        launcher = FindObjectOfType<Launcher>();
    }

    public void SetRoomInfo(RoomInfo _roomInfo)
    {
        roomInfo = _roomInfo;
        _text.text = _roomInfo.Name;
    }

    public void OnClick_JoinRoom()
    {
        launcher.SetRoomName(_text.text);
        launcher.JoinRoom();
    }
}
