using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PhotonConnectionMoniton : MonoBehaviourPunCallbacks
{
    [SerializeField] private SwitchableElement _disconectionWindow;
    [SerializeField] private Button[] _exitButtons;

    private void Awake()
    {
        foreach (var button in _exitButtons)
            button.onClick.AddListener(OnExitButtonClick);
    }

    private void OnDestroy()
    {
        foreach (var button in _exitButtons)
            button.onClick.RemoveListener(OnExitButtonClick);
    }

    private void OnExitButtonClick()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        _disconectionWindow.Enable();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _disconectionWindow.Enable();
    }
}
