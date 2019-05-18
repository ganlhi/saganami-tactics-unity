using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ST
{
    public class TurnPanel : MonoBehaviourPunCallbacks
    {
#pragma warning disable 0649
        [SerializeField] private TMP_Text turnNumber;
        [SerializeField] private TMP_Text stepName;
        [SerializeField] private Button readyButton;
        [SerializeField] private Sprite readyIcon;
        [SerializeField] private Sprite waitingIcon;
#pragma warning restore

        private void Start()
        {
            PlayController.Instance.OnStepStart.AddListener(UpdateUi);
            UpdateUi();

            readyButton.onClick.AddListener(() =>
            {
                PhotonNetwork.LocalPlayer.SetReady();
            });
        }

        private void UpdateUi()
        {
            turnNumber.text = "#" + PlayController.Instance.Turn;
            stepName.text = GetStepName(PlayController.Instance.Step);

            if (PhotonNetwork.LocalPlayer.IsReady() || PlayController.Instance.Busy)
            {
                readyButton.interactable = false;
                readyButton.GetComponent<Image>().sprite = waitingIcon;
            }
            else
            {
                readyButton.interactable = true;
                readyButton.GetComponent<Image>().sprite = readyIcon;
            }
        }

        private string GetStepName(TurnStep step)
        {
            switch (step)
            {
                case TurnStep.Start: return "Start";
                case TurnStep.Plotting: return "Plotting";
                case TurnStep.SetupSalvos: return "Targetting";
                case TurnStep.EarlySalvoImpact: return "Early Salvo";
                case TurnStep.HalfMove: return "Half Move";
                case TurnStep.MiddleSalvoImpact: return "Middle Salvo";
                case TurnStep.FirstBeamImpact: return "1st Beam";
                case TurnStep.FullMove: return "Full Move";
                case TurnStep.LateSalvoImpace: return "Late Salvo";
                case TurnStep.SecondBeamImpact: return "2nd Beam";
                case TurnStep.DamageControl: return "Damage Control";
                case TurnStep.End: return "End";
            }

            return string.Empty;
        }

        public override void OnPlayerPropertiesUpdate(Player target, Hashtable changedProps)
        {
            UpdateUi();
        }
    }
}