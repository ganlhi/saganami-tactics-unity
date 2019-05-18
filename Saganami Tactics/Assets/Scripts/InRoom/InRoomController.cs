using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ST
{
    public class InRoomController : MonoBehaviourPunCallbacks
    {
#pragma warning disable 0649
        [SerializeField] private Button leaveButton;
        [SerializeField] private Transform listContent;
        [SerializeField] private GameObject listEntryPrefab;
#pragma warning restore

        private void Start()
        {
            leaveButton.onClick.AddListener(LeaveRoom);
            FillList();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            AddPlayerToList(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            foreach (Transform child in listContent)
            {
                var entry = child.GetComponent<PlayerListEntry>();
                if (entry.Player == otherPlayer)
                {
                    Destroy(child.gameObject);
                    break;
                }
            }
        }

        public override void OnPlayerPropertiesUpdate(Player target, Hashtable changedProps)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                CheckForPlayersReady();
            }

            foreach (Transform child in listContent)
            {
                var entry = child.GetComponent<PlayerListEntry>();
                if (entry.Player == target)
                {
                    entry.UpdateUi(false);
                    break;
                }
            }
        }

        private void FillList()
        {
            foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                AddPlayerToList(player);
            }
        }

        private void AddPlayerToList(Player player)
        {
            var entry = Instantiate(listEntryPrefab).GetComponent<PlayerListEntry>();
            entry.Player = player;
            entry.transform.SetParent(listContent);
        }

        private void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(GameSettings.SceneLauncher);
        }

        private void CheckForPlayersReady()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount < PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                return;
            }

            if (PhotonNetwork.CurrentRoom.Players.Values.All(p => p.IsReady()))
            {
                PhotonNetwork.CurrentRoom.ResetPlayersReadiness();
                PhotonNetwork.LoadLevel(GameSettings.SceneSetup);
            }
        }
    }
}