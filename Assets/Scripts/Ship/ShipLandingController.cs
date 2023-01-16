using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipLandingController : MonoBehaviour
{
    #region Editor Fields

    [Header("Components")]
    [SerializeField] private Booster _booster;
    [SerializeField] private Collider[] _landingGearColliders;

    [Header("Landing Gear")]
    [SerializeField] private Transform _landingGearTransform;
    [SerializeField] private float _landingGearStoredYPos;
    [SerializeField] private float _landingGearDeployedYPos;

    #endregion

    #region Private Variables

    private enum LandingGearState
    {
        Up,
        Between,
        Down
    }
    private LandingGearState _isWantedDeployed = LandingGearState.Down;
    private LandingGearState _isLandingGearDeployed = LandingGearState.Down;
    private LandingGearState _previousState = LandingGearState.Down;

    #endregion

    #region Unity Loops

    private void Start()
    {
        PlayerInputHandler.OnSpecialAction += HandleSpecialAction;
    }

    private void FixedUpdate()
    {
        float wantedYPosition = 0;

        if (_isWantedDeployed == LandingGearState.Down)
        {
            wantedYPosition = _landingGearStoredYPos;
        }
        else if (_isWantedDeployed == LandingGearState.Up)
        {
            wantedYPosition = _landingGearDeployedYPos;
        }
        else
        {
            wantedYPosition = _landingGearTransform.localPosition.y;
        }

        _landingGearTransform.localPosition = new
            Vector3(_landingGearTransform.localPosition.x,
            Mathf.Lerp(_landingGearTransform.localPosition.y, wantedYPosition, Time.deltaTime), _landingGearTransform.localPosition.z);

        if (_isWantedDeployed != LandingGearState.Between)
        {
            if (_landingGearTransform.localPosition.y <= _landingGearDeployedYPos + 0.3f && _isWantedDeployed == LandingGearState.Up) { _landingGearTransform.localPosition = new Vector3(_landingGearTransform.localPosition.x, _landingGearDeployedYPos, _landingGearTransform.localPosition.z); _isLandingGearDeployed = LandingGearState.Up; }
            else if (_landingGearTransform.localPosition.y >= _landingGearStoredYPos - 0.3f && _isWantedDeployed == LandingGearState.Down) { _landingGearTransform.localPosition = new Vector3(_landingGearTransform.localPosition.x, _landingGearStoredYPos, _landingGearTransform.localPosition.z); _isLandingGearDeployed = LandingGearState.Down; }
        }
        else
        {
            _isLandingGearDeployed = LandingGearState.Between;
        }
    }

    private void OnDestroy()
    {
        PlayerInputHandler.OnSpecialAction -= HandleSpecialAction;
    }

    #endregion

    private void HandleSpecialAction(PlayerInputHandler player, bool isPressed)
    {
        PlayerInteractionController playerInteractionController = _booster.CurrentPlayer as PlayerInteractionController;

        //If player that pressed action button is not on booster, return
        if (player != playerInteractionController?.PlayerInput) { return; }

        if (_isWantedDeployed == _isLandingGearDeployed && _isWantedDeployed != LandingGearState.Between) { _previousState = _isWantedDeployed; _isWantedDeployed = ReverseState(_isWantedDeployed); }
        else if (_isWantedDeployed != _isLandingGearDeployed) { _previousState = _isWantedDeployed; _isWantedDeployed = LandingGearState.Between; }
        else if (_isWantedDeployed == _isLandingGearDeployed && _isWantedDeployed == LandingGearState.Between) { _isWantedDeployed = ReverseState(_previousState); }
    }

    private LandingGearState ReverseState(LandingGearState state)
    {
        if (state == LandingGearState.Up) { state = LandingGearState.Down; }
        else if (state == LandingGearState.Down) { state = LandingGearState.Up; }

        return state;
    }
}