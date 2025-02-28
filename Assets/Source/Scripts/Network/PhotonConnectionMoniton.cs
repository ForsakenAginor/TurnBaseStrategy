using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonConnectionMoniton : MonoBehaviourPunCallbacks
{
    [SerializeField] private SwitchableElement _disconectionWindow;

    public override void OnDisconnected(DisconnectCause cause)
    {
        _disconectionWindow.Enable();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _disconectionWindow.Enable();
    }
}
