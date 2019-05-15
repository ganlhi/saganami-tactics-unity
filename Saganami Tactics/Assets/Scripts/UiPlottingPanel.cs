using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiPlottingPanel : MonoBehaviour
{
    private GameController ctrl { get { return GameController.Instance; } }

#pragma warning disable 0649
    [SerializeField] TMP_Text pivotCounter;
    [SerializeField] TMP_Text rollCounter;
    [SerializeField] TMP_Text thrustCounter;
    [SerializeField] Slider thrustSlider;
    [SerializeField] Button[] diagonalButtons;
#pragma warning restore

    public void UndoPivot()
    {
        ctrl.SelectedShip?.Plotting.UndoPivot();
    }
    public void UndoRoll()
    {
        ctrl.SelectedShip?.Plotting.UndoRoll();
    }

    public void PlotPivot(int index)
    {
        Pivot p = (Pivot)index;
        ctrl.SelectedShip?.PlotPivot(p);
    }
    public void PlotRoll(int index)
    {
        Roll r = (Roll)index;
        ctrl.SelectedShip?.PlotRoll(r);
    }

    public void PlotThrust(float thr)
    {
        ctrl.SelectedShip?.PlotThrust(Mathf.FloorToInt(thr));
    }

    private void Update()
    {
        var visible = ctrl.IsPlotting && ctrl.SelectedShip != null;
        GetComponent<Image>().enabled = visible;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(visible);
        }

        if (visible)
        {
            updateCounters();
        }
    }

    private void updateCounters()
    {
        pivotCounter.text = string.Format("{0} / {1}", ctrl.SelectedShip.AvailablePivots, ctrl.SelectedShip.Stats.MaxPivots);
        rollCounter.text = string.Format("{0} / {1}", ctrl.SelectedShip.AvailableRolls, ctrl.SelectedShip.Stats.MaxRolls);
        thrustCounter.text = string.Format("{0}", ctrl.SelectedShip.Plotting.Thrust);

        thrustSlider.maxValue = ctrl.SelectedShip.Stats.MaxThrust;

        foreach (var btn in diagonalButtons)
        {
            btn.interactable = ctrl.SelectedShip.Plotting.CanPivotOnDiagonals;
        }
    }
}
