using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ST
{
    public class RoomsListEntry : MonoBehaviour
    {

        public RoomInfo Room;

#pragma warning disable 0649
        [SerializeField] private TMP_Text roomName;
        [SerializeField] private TMP_Text players;
        [SerializeField] private Button joinButton;
#pragma warning restore

        public UnityEvent JoinEvent { get { return joinButton.onClick; } }

        private void Start()
        {
            roomName.text = Room.Name;
            players.text = Room.PlayerCount + " / " + Room.MaxPlayers;
        }
    }
}