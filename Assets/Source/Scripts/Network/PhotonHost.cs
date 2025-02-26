using Assets.Scripts.General;
using NUnit.Framework;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotonHost : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button _createRoomButton;
    [SerializeField] private Button _cancel;
    [SerializeField] private SwitchableElement _createRoomWindow;
    [SerializeField] private SwitchableElement _holderWindow;
    [SerializeField] private TMP_Text _connectingStatusTextField;

    private bool _isActive = false;
    private bool _isRoomCreated = false;
    private List<RoomInfo> _rooms;
    private string _roomName;

    private void Awake()
    {
        _createRoomButton.onClick.AddListener(OnCreateRoomButtonClick);
        _cancel.onClick.AddListener(OnCancelButtonClick);
    }

    private void OnDestroy()
    {
        _createRoomButton.onClick.RemoveListener(OnCreateRoomButtonClick);
        _cancel.onClick.RemoveListener(OnCancelButtonClick);
    }

    public override void OnConnectedToMaster()
    {
        if (_isActive)
            PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        if (_isActive)
            PhotonNetwork.GetCustomRoomList(TypedLobby.Default, null);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (_isActive && _isRoomCreated == false)
        {
            _rooms = roomList;
            CreateRoom();
        }
    }

    public override void OnJoinedRoom()
    {
        if (_isActive)
        {
            _connectingStatusTextField.text = $"Room created\n{_roomName}";
            _isRoomCreated = true;
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.SendRate = 10;
            PhotonNetwork.SerializationRate = 10;
        }
    }

    public override void OnPlayerEnteredRoom(Player _)
    {
        if (_isActive)
        {
            SceneChangerSingleton.Instance.LoadSceneInMuliplayer(Scenes.PhotonScene.ToString());
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (_isActive)
            _connectingStatusTextField.text = "Disconnected";
    }

    private void CreateRoom()
    {
        _connectingStatusTextField.text = "Creating room...";
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = false;
        roomOptions.MaxPlayers = 2;
        _roomName = GetRandomRoomNumber();
        PhotonNetwork.CreateRoom(_roomName, roomOptions, TypedLobby.Default);
    }

    private void OnCreateRoomButtonClick()
    {
        _isActive = true;
        _holderWindow.Disable();
        _createRoomWindow.Enable();
        _connectingStatusTextField.text = "Connecting...";
        PhotonNetwork.ConnectUsingSettings();
    }

    private void OnCancelButtonClick()
    {
        _isActive = false;
        _isRoomCreated = false;
        PhotonNetwork.Disconnect();
        _createRoomWindow.Disable();
        _holderWindow.Enable();
    }

    private string GetRandomRoomNumber()
    {
        int seed = Random.Range(10000, 99999);

        while (_rooms.Exists(o => o.Name == seed.ToString()))
            seed = Random.Range(10000, 99999);

        return seed.ToString();
    }
}
