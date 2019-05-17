using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviourPunCallbacks
{
    GameObject menu;

    private void Awake()
    {
        menu = GameObject.Find("Menu");

        PhotonNetwork.AutomaticallySyncScene = true;
    }
    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            menu.SetActive(false);
        }
    }

    public void CreateGame()
    {
        PhotonNetwork.CreateRoom("Test");
        menu.SetActive(false);
    }

    public void JoinGame()
    {
        PhotonNetwork.JoinRoom("Test");
        menu.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        menu.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        SceneManager.LoadScene("Game");
    }
}
