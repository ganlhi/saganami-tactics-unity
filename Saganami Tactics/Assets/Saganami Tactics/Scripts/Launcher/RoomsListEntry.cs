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

        [SerializeField]
        private TMP_Text roomName;

        [SerializeField]
        private TMP_Text players;

        [SerializeField]
        private TMP_Text maxPoints;

        [SerializeField]
        private Button joinButton;

        public UnityEvent JoinEvent { get { return joinButton.onClick; } }

        private void Start()
        {
            roomName.text = Room.Name;
            players.text = Room.PlayerCount + " / " + Room.MaxPlayers;
            maxPoints.text = Room.GetMaxPoints().ToString();
        }
    }
}