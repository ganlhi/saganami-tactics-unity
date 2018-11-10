using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ST
{
    public class TurnPanel : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text turnNumber;

        [SerializeField]
        private TMP_Text stepName;

        [SerializeField]
        private Button readyButton;

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
    }
}