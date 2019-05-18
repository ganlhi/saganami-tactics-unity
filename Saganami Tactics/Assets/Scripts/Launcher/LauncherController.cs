using Photon.Pun;
using UnityEngine;

namespace ST
{
    public class LauncherController : MonoBehaviourPunCallbacks
    {
#pragma warning disable 0649
        [SerializeField] private GameObject startPanel;
        [SerializeField] private GameObject createPanel;
        [SerializeField] private GameObject joinPanel;
#pragma warning restore

        private void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.ConnectUsingSettings();
            }

            SetMode(StartMode);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetMode(StartMode);
            }
        }

        public override void OnConnectedToMaster()
        {
            Debug.LogFormat("<color=blue>Connected to master</color> Ready: {0}", PhotonNetwork.IsConnectedAndReady);
        }

        public static readonly int StartMode = 0;
        public static readonly int CreateMode = 1;
        public static readonly int JoinMode = 2;

        public void SetMode(int mode)
        {
            startPanel.SetActive(mode == StartMode);
            createPanel.SetActive(mode == CreateMode);
            joinPanel.SetActive(mode == JoinMode);
        }

        public void QuitGame()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }

    }
}