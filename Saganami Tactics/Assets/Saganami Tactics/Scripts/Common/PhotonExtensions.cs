using ExitGames.Client.Photon;
using Photon.Realtime;

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
            player.SetCustomProperties(new Hashtable() {
                { GameSettings.ReadyProp, newReady },
            });
        }
    }

    public static class RoomInfoExtensions
    {
        public static int GetMaxPoints(this RoomInfo room)
        {
            object points;
            if (room.CustomProperties.TryGetValue(GameSettings.MaxPointsProp, out points))
            {
                return (int)points;
            }
            return 0;
        }
    }
}