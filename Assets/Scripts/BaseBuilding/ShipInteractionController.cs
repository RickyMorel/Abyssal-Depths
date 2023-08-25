using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInteractionController : MonoBehaviour
{
    #region Private Variables

    private bool _isInteracting;
    private BuildingInteractable _currentInteractable;

    #endregion

    #region Public Properties

    public PlayerInputHandler CurrentDriver => Ship.Instance?.ShipLandingController?.Booster?.CurrentPlayer ?
        Ship.Instance.ShipLandingController.Booster.CurrentPlayer.PlayerInput : null;

    #endregion

    #region Unity Loops

    private void Update()
    {
        if(CurrentDriver == null) { return; }

        if(_currentInteractable == null) { return; }

        if (!CurrentDriver.IsShooting_2) { return; }

        if (_isInteracting) { return; }

        _isInteracting = true;

        _currentInteractable.Interact();
    }

    private void HandleTryUpgrade()
    {
        if ((_currentInteractable is BuildingUpgradable) == false) { return; }

        BuildingUpgradable upgradable = _currentInteractable as BuildingUpgradable;

        upgradable.TryUpgrade();
    }

    #endregion

    public void SetCurrentInteractable(BuildingInteractable building)
    {
        _currentInteractable = building;

        if (building == null) { _isInteracting = false; CurrentDriver.OnUpgrade -= HandleTryUpgrade; }
        else { CurrentDriver.OnUpgrade += HandleTryUpgrade; }
    }
}
