using Assets.Scripts.General;
using Lean.Localization;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotonConnect : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button _enterToRoomButton;
    [SerializeField] private Button _connectButton;
    [SerializeField] private Button _cancel;
    [SerializeField] private SwitchableElement _connectRoomWindow;
    [SerializeField] private SwitchableElement _holderWindow;
    [SerializeField] private TMP_Text _connectingStatusTextField;
    [SerializeField] private TMP_InputField _roomNumberInputField;

    private bool _isActive = false;

    private void Awake()
    {
        _connectButton.onClick.AddListener(OnConnectButtonClick);
        _cancel.onClick.AddListener(OnCancelButtonClick);
        _enterToRoomButton.interactable = false;
    }

    private void OnDestroy()
    {
        _connectButton.onClick.RemoveListener(OnConnectButtonClick);
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
        {
            string status = LeanLocalization.GetTranslationText("EnterRoomCode");
            _connectingStatusTextField.text = status;
            _enterToRoomButton.interactable = true;
            _enterToRoomButton.onClick.AddListener(OnEnterButtonClick);
        }
    }

    public override void OnJoinedRoom()
    {
        if (_isActive)
        {
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.SendRate = 10;
            PhotonNetwork.SerializationRate = 10;
            SceneChangerSingleton.Instance.LoadSceneInMuliplayer(Scenes.PhotonScene.ToString());
        }
    }

    public override void OnJoinRoomFailed(short _, string _1)
    {
        if (_isActive)
        {
            _roomNumberInputField.text = string.Empty;
            _enterToRoomButton.interactable = true;
            _roomNumberInputField.interactable = true;
            _enterToRoomButton.onClick.AddListener(OnEnterButtonClick);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (_isActive)
        {
            string status = LeanLocalization.GetTranslationText("Disconnected");
            _connectingStatusTextField.text = status;
        }
    }

    private void OnEnterButtonClick()
    {
        _enterToRoomButton.onClick.RemoveAllListeners();
        _enterToRoomButton.interactable = false;
        _roomNumberInputField.interactable = false;
        PhotonNetwork.JoinRoom(_roomNumberInputField.text);
    }

    private void OnConnectButtonClick()
    {
        _isActive = true;
        _holderWindow.Disable();
        _connectRoomWindow.Enable();
        string status = LeanLocalization.GetTranslationText("Connecting");
        _connectingStatusTextField.text = status;
        PhotonNetwork.ConnectUsingSettings();
    }

    private void OnCancelButtonClick()
    {
        _isActive = false;
        PhotonNetwork.Disconnect();
        _enterToRoomButton.interactable = false;
        _enterToRoomButton.onClick.RemoveAllListeners();
        _connectRoomWindow.Disable();
        _holderWindow.Enable();
    }
}