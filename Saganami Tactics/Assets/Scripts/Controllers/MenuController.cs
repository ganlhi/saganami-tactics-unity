using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviourPunCallbacks
{
    GameObject menu;

    private void Start()
    {
        menu = GameObject.Find("Menu");
        menu.SetActive(false);

        PhotonNetwork.ConnectUsingSettings();
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
