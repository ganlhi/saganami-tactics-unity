using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ST
{
    public class PlayerListEntry : MonoBehaviour
    {
        #region Private Constants

        private const string playerNamePrefKey = "PlayerName";

        #endregion Private Constants

        #region Editor customization

        [SerializeField]
        private TMP_InputField nicknameInput;

        [SerializeField]
        private Image nicknameInputValid;

        [SerializeField]
        private TMP_Text nicknameLabel;

        [SerializeField]
        private Image playerImage;

        [SerializeField]
        private Button colorButton;

        [SerializeField]
        private Button readyButton;

        [SerializeField]
        private Image readyImage;

        [SerializeField]
        private Sprite localPlayerIcon;

        [SerializeField]
        private Sprite distantPlayerIcon;

        [SerializeField]
        private Sprite notReadyIcon;

        [SerializeField]
        private Sprite readyIcon;

        #endregion Editor customization

        #region Public variables

        public Player Player;

        #endregion Public variables

        #region Unity callbacks

        private void Start()
        {
            LoadSavedPlayerName();
            SetupUi();
            UpdateUi();
        }

        #endregion Unity callbacks

        #region Private methods

        private void LoadSavedPlayerName()
        {
            if (Player.IsLocal)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    Player.NickName = PlayerPrefs.GetString(playerNamePrefKey);
                }
            }
        }

        private void SetupUi()
        {
            readyButton.gameObject.SetActive(Player.IsLocal);
            nicknameInput.gameObject.SetActive(Player.IsLocal);
            nicknameLabel.gameObject.SetActive(!Player.IsLocal);
            playerImage.sprite = Player.IsLocal ? localPlayerIcon : distantPlayerIcon;
            colorButton.interactable = Player.IsLocal;

            if (Player.IsLocal)
            {
                nicknameInput.onValueChanged.AddListener(name =>
                {
                    Player.NickName = name;
                    PlayerPrefs.SetString(playerNamePrefKey, name);
                });

                readyButton.onClick.AddListener(() =>
                {
                    Player.ToggleReady();
                });

                colorButton.onClick.AddListener(() =>
                {
                    Player.CycleColorIndex();
                });
            }
        }

        #endregion Private methods

        #region Public methods

        public void UpdateUi(bool resetNameInput = true)
        {
            colorButton.GetComponent<Image>().color = GameSettings.GetColor(Player.GetColorIndex());
            if (resetNameInput)
            {
                nicknameInput.text = Player.NickName;
            }
            nicknameInputValid.gameObject.SetActive(
                !string.IsNullOrEmpty(Player.NickName) && nicknameInput.text == Player.NickName
                );
            nicknameLabel.text = Player.NickName;
            readyImage.sprite = Player.IsReady() ? readyIcon : notReadyIcon;
            readyButton.interactable = !string.IsNullOrEmpty(Player.NickName) && Player.GetColorIndex() >= 0;
        }

        #endregion Public methods
    }
}