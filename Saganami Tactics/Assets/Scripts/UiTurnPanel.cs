using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UiTurnPanel : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private TMP_Text text;
    [SerializeField] private Button button;
#pragma warning restore

    private void Start()
    {
        button.onClick.AddListener(GameController.Instance.NextStep);
    }

    private void Update()
    {
        var ctrl = GameController.Instance;

        text.text = string.Format("{0} - {1}", ctrl.Turn, ctrl.Step);
        button.interactable = ctrl.CanGoNextStep;
    }
}
