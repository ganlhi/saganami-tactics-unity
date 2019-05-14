using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiPlottingPanel : MonoBehaviour
{
    private GameController ctrl { get { return GameController.Instance; } }

    public void ResetPivot()
    {

    }
    public void ResetRoll()
    {

    }

    public void PlotThrust(float thr)
    {

    }

    private void Update()
    {
        var visible = ctrl.Step == Step.Plotting && ctrl.Started;
        GetComponent<Image>().enabled = visible;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(visible);
        }
    }
}
