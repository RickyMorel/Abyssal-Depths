using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpgradesController : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject _chipPickupPrefab;
    [SerializeField] private Transform _handTransform;

    #endregion

    #region Private Variables

    private PlayerInputHandler _playerInput;
    private BaseInteractionController _interactionController;
    private PlayerCarryController _playerCarryController;

    #endregion

    private void Start()
    {
        _playerInput = GetComponent<PlayerInputHandler>();
        _playerCarryController = GetComponent<PlayerCarryController>();
        _interactionController = GetComponent<BaseInteractionController>();

        _chipPickupPrefab = GameAssetsManager.Instance.ChipPickup;

        _playerInput.OnUpgrade += HandleInteractableTinker;
    }

    private void OnDestroy()
    {
        _playerInput.OnUpgrade -= HandleInteractableTinker;
    }

    private void HandleInteractableTinker()
    {
        if (_interactionController.CurrentInteractable == null) { return; }

        //If item needs to be fixed, fix first
        if (HandleFix()) { return; }

        if (_playerCarryController.CurrentSingleItem == null) { return; }

        if (_playerCarryController.CurrentSingleItem is UpgradeChip)
        {
            HandleUpgrade();
        }
        else if(_playerCarryController.CurrentSingleObjInstance.tag == "RemovalTool")
        {
            HandleRemoveUpgrades();
        }
    }

    private void HandleRemoveUpgrades()
    {
        if ((_interactionController.CurrentInteractable is Upgradable) == false) { return; }

        Upgradable upgradable = _interactionController.CurrentInteractable as Upgradable;

        upgradable.RemoveUpgrades();

        _playerCarryController.CurrentSingleItem = null;
        Destroy(_playerCarryController.CurrentSingleObjInstance.gameObject);
    }


    //This calls when the player presses the upgrade button
    private void HandleUpgrade()
    {
        if ((_interactionController.CurrentInteractable is Upgradable) == false) { return; }

        Upgradable upgradable = _interactionController.CurrentInteractable as Upgradable;
        UpgradeChip upgradeItem = _playerCarryController.CurrentSingleItem as UpgradeChip;

        bool didUpgrade = _playerCarryController.CurrentSingleItem is UpgradeChip ? upgradable.TryUpgrade(upgradeItem, _playerCarryController) : false;

        if (didUpgrade)
        {
            _playerCarryController.CurrentSingleItem = null;
            Destroy(_playerCarryController.CurrentSingleObjInstance.gameObject);
        }
    }

    private bool HandleFix()
    {
        if (!_interactionController.CurrentInteractable.IsBroken()) { HandleUpgrade(); return false; }

        if (_interactionController.IsInteracting()) { return false; }

        _interactionController.IsFixing = true;

        _interactionController.HandleInteraction();

        return true;
    }
}
