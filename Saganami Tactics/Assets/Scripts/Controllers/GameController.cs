using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviourPunCallbacks
{
    public static GameController Instance;

    public float AutoMoveDuration = 2;
    public Side LocalPlayerSide = Side.Red;
    public Ship SelectedShip;
    public bool Started;
    public int Turn;
    public Step Step;
    public bool CanGoNextStep = true;

    public List<Ship> Ships => new List<Ship>(ships.Values);

    public bool IsPlotting => Started && Step == Step.Plotting;

#pragma warning disable 0649
    [SerializeField] GameState initialState;
#pragma warning restore

    // TODO move to SetupController
    /*
    private Dictionary<Side, Vector3> sidesDeploymentOrigins = new Dictionary<Side, Vector3>()
    {
        { Side.Red, new Vector3(25, 0, 0) },
        { Side.Blue, new Vector3(-25, 0, 0) },
        { Side.Yellow, new Vector3(0, 0, 25) },
        { Side.Green, new Vector3(0, 0, -25) },
    };
    */

    private Dictionary<Guid, Ship> ships;

    #region Startup

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Only one instance allowed");
            Destroy(this);
            return;
        }
        else
        {
            Instance = this;
        }

        ships = new Dictionary<Guid, Ship>();
    }
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            loadInitialState();
        }
        else if (!PhotonNetwork.InRoom)
        {
            SceneManager.LoadScene("Menu");
        }
    }

    private void loadInitialState()
    {
        if (initialState == null) return;

        foreach (ShipState s in initialState.Ships)
        {
            var ship = PhotonNetwork.Instantiate("Prefabs/Ship", Vector3.zero, Quaternion.identity).GetComponent<Ship>();
            ship.ID = Guid.NewGuid();

            ship.Side = s.Side;
            ship.Name = s.Name;
            ship.Stats = s.Stats;
            ship.Position = s.Position;
            ship.Velocity = s.Velocity;
            ship.Attitude = s.Attitude;

            ship.MarkersDisplayMode = LocalPlayerSide == s.Side ? Marker.DisplayMode.Ghost : Marker.DisplayMode.Dot;

            ships.Add(ship.ID, ship);
        }
    }

    #endregion

    #region Steps

    public void NextStep()
    {
        if (!Started)
        {
            Started = true;
            Turn = 1;
            Step = Step.Plotting;
            placeShipsMarkers();
            return;
        }

        switch (Step)
        {
            case Step.Plotting:
                applyDisplacements();
                Step = Step.SetupSalvos;
                break;
            case Step.SetupSalvos:
                Step = Step.EarlySalvo;
                break;
            case Step.EarlySalvo:
                Step = Step.HalfMove;
                moveShipsToNextMarker();
                StartCoroutine(waitForAutoMove(Step.MiddleSalvo));
                break;
            case Step.HalfMove:
                Debug.LogError("Should not happen via this method call");
                break;
            case Step.MiddleSalvo:
                Step = Step.FirstBeamImpact;
                break;
            case Step.FirstBeamImpact:
                Step = Step.FullMove;
                moveShipsToNextMarker();
                StartCoroutine(waitForAutoMove(Step.LateSalvo));
                break;
            case Step.FullMove:
                Debug.LogError("Should not happen via this method call");
                break;
            case Step.LateSalvo:
                Step = Step.SecondBeamImpact;
                break;
            case Step.SecondBeamImpact:
                Step = Step.EndOfTurn;
                applyThrust();
                break;
            case Step.EndOfTurn:
                Step = Step.Plotting;
                Turn += 1;
                placeShipsMarkers();
                break;
        }
    }

    private IEnumerator waitForAutoMove(Step nextStep)
    {
        CanGoNextStep = false;
        yield return new WaitForSeconds(AutoMoveDuration);
        CanGoNextStep = true;
        Step = nextStep;
    }

    private void placeShipsMarkers()
    {
        Ships.ForEach(s => s.PlaceMarkers());
    }

    private void applyThrust()
    {
        Ships.ForEach(s => s.ApplyThrust());
    }

    private void applyDisplacements()
    {
        Ships.ForEach(s => s.ApplyDisplacement());
    }

    private void moveShipsToNextMarker()
    {
        Ships.ForEach(s => s.MoveToNextMarker());
    }

    #endregion

    #region Network



    #endregion
}
