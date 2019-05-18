using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;

namespace ST
{
    public static class PlayerExtensions
    {
        public static int GetColorIndex(this Player player)
        {
            object idx;
            if (player.CustomProperties.TryGetValue(GameSettings.ColorIndexProp, out idx))
            {
                return (int)idx;
            }
            else
            {
                return -1;
            }
        }

        public static void CycleColorIndex(this Player player)
        {
            var index = (player.GetColorIndex() + 1) % 4;
            player.SetCustomProperties(new Hashtable() {
                { GameSettings.ColorIndexProp, index },
            });
        }

        public static bool IsReady(this Player player)
        {
            object rdy;
            if (player.CustomProperties.TryGetValue(GameSettings.ReadyProp, out rdy))
            {
                return (bool)rdy;
            }
            else
            {
                return false;
            }
        }

        public static void ToggleReady(this Player player)
        {
            var newReady = !player.IsReady();
            player.SetReady(newReady);
        }

        public static void SetReady(this Player player, bool ready = true)
        {
            player.SetCustomProperties(new Hashtable() {
                { GameSettings.ReadyProp, ready },
            });
        }

        public static List<Ship> GetShips(this Player player)
        {
            return PhotonNetwork
                .FindGameObjectsWithComponent(typeof(Ship))
                .Select(go => go.GetComponent<Ship>())
                .Where(ship =>
                {
                    return ship.photonView != null && ship.photonView.Owner == player;
                })
                .ToList();
        }
    }

    public static class RoomExtensions
    {
        public static void ResetPlayersReadiness(this Room room)
        {
            foreach (Player p in room.Players.Values)
            {
                p.SetReady(false);
            }
        }
    }
}