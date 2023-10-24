using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BuildingHealth))]
public class BuildingGate : BuildingUpgradable
{
    #region Editor Fields

    [SerializeField] private Transform _gateTransform;
    [SerializeField] private float _closedYPosition;
    [SerializeField] private float _openYPosition;

    #endregion

    #region Private Varaibles

    private GateState _currentGateState = GateState.Closed;
    private GateState _wantedGateState = GateState.Closed;
    private float _gateYPosition;

    #endregion

    #region Unity Loops

    private void FixedUpdate()
    {
        if (_wantedGateState == _currentGateState) { return; }

        ChangeDoorState();
    }

    private void ChangeDoorState()
    {
        float wantedYPosition = _currentGateState == GateState.Open ? _closedYPosition : _openYPosition;

        _gateYPosition = Mathf.Lerp(_gateTransform.localPosition.y, wantedYPosition, Time.deltaTime);

        _gateTransform.localPosition = new Vector3(_gateTransform.localPosition.x, _gateYPosition, _gateTransform.localPosition.z);

        if (Mathf.RoundToInt(_gateTransform.localPosition.y) == Mathf.RoundToInt(wantedYPosition)) { _currentGateState = _wantedGateState; }
    }
    #endregion

    public override bool Interact()
    {
        Debug.Log("Interact");

        if (_buildingHealth.IsDead()) { return false; }

        //Dont interact with gate while its moving
        if (_currentGateState != _wantedGateState) { return false; }

        Debug.Log("_currentGateState == _wantedGateState");

        _wantedGateState = _wantedGateState == GateState.Open ? GateState.Closed : GateState.Open;

        GameAudioManager.Instance.PlaySound(GameAudioManager.Instance.ShipDoorOpenCloseSfx, transform.position);

        base.Interact();

        return false;
    }

    private enum GateState
    {
        Open,
        Closed
    }
}
