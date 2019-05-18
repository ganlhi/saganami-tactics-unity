using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace ST
{
    public class JoinPanelController : MonoBehaviourPunCallbacks
    {
#pragma warning disable 0649
        [SerializeField] private Transform listContent;
        [SerializeField] private GameObject listEntryPrefab;
#pragma warning restore

        private new void OnEnable()
        {
            base.OnEnable();
            PhotonNetwork.JoinLobby();
        }

        private new void OnDisable()
        {
            base.OnDisable();
            PhotonNetwork.LeaveLobby();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("<color=blue>Connected to lobby</color>");
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.LogFormat("Rooms number: {0}", roomList.Count);

            // Empty current list
            foreach (Transform child in listContent)
            {
                child.GetComponent<RoomsListEntry>().JoinEvent.RemoveAllListeners();
                Destroy(child.gameObject);
            }

            // Fill with new list items
            foreach (RoomInfo ri in roomList)
            {
                var entry = Instantiate(listEntryPrefab).GetComponent<RoomsListEntry>();
                entry.Room = ri;
                entry.JoinEvent.AddListener(() => { PhotonNetwork.JoinRoom(ri.Name); });
                entry.transform.SetParent(listContent);
            }
        }
    }
}