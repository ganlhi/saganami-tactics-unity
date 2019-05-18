using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ST
{
    public class CreatePanelController : MonoBehaviourPunCallbacks
    {
#pragma warning disable 0649
        [SerializeField] private TMP_InputField gameNameField;
        [SerializeField] private Slider playersNumberSlider;
        [SerializeField] private Button createButton;
#pragma warning restore

        private void Update()
        {
            createButton.interactable = !string.IsNullOrEmpty(gameNameField.text) && PhotonNetwork.IsConnectedAndReady;
        }

        public void CreateRoom()
        {
            var roomName = gameNameField.text;

            PhotonNetwork.CreateRoom(roomName, new RoomOptions()
            {
                MaxPlayers = (byte)playersNumberSlider.value
            });
        }

        public override void OnJoinedRoom()
        {
            Debug.LogFormat("<color=blue>Room created and joined:</color> {0}", PhotonNetwork.CurrentRoom.Name);
        }

        public override void OnPlayerPropertiesUpdate(Player player, Hashtable changedProps)
        {
            if (!player.IsLocal)
            {
                return;
            }

            Debug.LogFormat("<color=purple>Player props initialized:</color> ColorIndex={0} Ready={1}",
                player.GetColorIndex(),
                player.IsReady());

            PhotonNetwork.LoadLevel(GameSettings.SceneInRoom);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogErrorFormat("<color=red>Could not create room:</color> {0}", message);
        }
    }
}
