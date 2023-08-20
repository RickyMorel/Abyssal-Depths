using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipLandingController : MonoBehaviour
{
    #region Editor Fields

    [Header("Components")]
    [SerializeField] private Booster _booster;

    [Header("Landing Gear")]
    [SerializeField] private Transform _landingGearTransform;
    [SerializeField] private float _landingGearStoredYPos;
    [SerializeField] private float _landingGearDeployedYPos;

    #endregion

    #region Private Variables

    private LandingGearState _isWantedDeployed = LandingGearState.Down;
    private LandingGearState _isLandingGearDeployed = LandingGearState.Down;
    private LandingGearState _previousState = LandingGearState.Down;
    private float _offsetLandingGearPosition = 0.3f;

    #endregion

    #region Public Properties

    public Booster Booster => _booster;

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
            //When the landing gear is  deploying, it never actually reaches the StoredYPos nor the DeployedYPos, so in order to offset that, check that the landing gear has almost reached its destination, have it snap to its desired location.
            //To decide when to snap it, use the _offset variable, the smaller the number, it appears smoother, but it also takes longer as the way the landing currently works makes it so that the closer it is to its destination, the slower it gets.
            if (_landingGearTransform.localPosition.y <= _landingGearDeployedYPos + _offsetLandingGearPosition && _isWantedDeployed == LandingGearState.Up) { _landingGearTransform.localPosition = new Vector3(_landingGearTransform.localPosition.x, _landingGearDeployedYPos, _landingGearTransform.localPosition.z); _isLandingGearDeployed = LandingGearState.Up; }
            else if (_landingGearTransform.localPosition.y >= _landingGearStoredYPos - _offsetLandingGearPosition && _isWantedDeployed == LandingGearState.Down) { _landingGearTransform.localPosition = new Vector3(_landingGearTransform.localPosition.x, _landingGearStoredYPos, _landingGearTransform.localPosition.z); _isLandingGearDeployed = LandingGearState.Down; }
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

    private void HandleSpecialAction(PlayerInputHandler player)
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

    private enum LandingGearState
    {
        Up,
        Between,
        Down
    }
}