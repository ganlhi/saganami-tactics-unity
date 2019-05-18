using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class StartPanelController : MonoBehaviourPun
{
#pragma warning disable 0649
    [SerializeField] private Button createButton;
    [SerializeField] private Button joinButton;
#pragma warning restore

    private void FixedUpdate()
    {
        createButton.interactable = PhotonNetwork.IsConnectedAndReady;
        joinButton.interactable = PhotonNetwork.IsConnectedAndReady;
    }
}
