using UnityEngine;

namespace ST
{
    public static class GameSettings
    {
        #region Custom properties keys
        public static readonly string ColorIndexProp = "cIdx";
        public static readonly string ReadyProp = "rdy";
        public static readonly string MaxPointsProp = "pts";
        #endregion

        #region Scenes names
        public static readonly string SceneLauncher = "Launcher";
        public static readonly string SceneInRoom = "InRoom";
        public static readonly string SceneSetup = "Setup";
        public static readonly string SceneDeploy = "Deploy";
        public static readonly string ScenePlay = "Play";
        #endregion


        #region Utility functions
        public static Color GetColor(int colorIndex)
        {
            switch (colorIndex)
            {
                case 0: return Color.red;
                case 1: return Color.blue;
                case 2: return Color.green;
                case 3: return Color.yellow;
                default: return Color.white;
            }
        }

        public static Vector3 GetDeploymentCenterPoint(int colorIndex)
        {
            switch (colorIndex)
            {
                case 0: return new Vector3(-30, 0, 0);
                case 1: return new Vector3(30, 0, 0);
                case 2: return new Vector3(0, 0, -30);
                case 3: return new Vector3(0, 0, 30);
                default: return Vector3.zero;
            }
        }
        #endregion
    }
}