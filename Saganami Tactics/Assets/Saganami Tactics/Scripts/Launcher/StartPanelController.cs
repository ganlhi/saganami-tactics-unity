using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class StartPanelController : MonoBehaviourPun
{
    [SerializeField]
    private Button createButton;

    [SerializeField]
    private Button joinButton;

    private void FixedUpdate()
    {
        createButton.interactable = PhotonNetwork.IsConnectedAndReady;
        joinButton.interactable = PhotonNetwork.IsConnectedAndReady;
    }
}
