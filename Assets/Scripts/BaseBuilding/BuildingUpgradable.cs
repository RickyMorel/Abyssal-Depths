using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingUpgradable : BuildingInteractable
{
    #region Editor Fields

    [SerializeField] private Upgrade[] _upgrades;

    #endregion

    #region Private Variables

    private int _currentUpgradeIndex = 0;

    #endregion

    public override void Start()
    {
        base.Start();

        SetToCurrentUpgrade();
    }

    public override void Interact()
    {
        base.Interact();

        BuildingUpgradeUI.Instance.Initialize(GetCurrentUpgrade().CraftingRecipy);
    }

    public override void Uninteract()
    {
        base.Uninteract();

        BuildingUpgradeUI.Instance.EnableUpgradesPanel(false);
    }

    private void SetToCurrentUpgrade()
    {
        foreach (Upgrade upgrade in _upgrades)
        {
            upgrade.Mesh.SetActive(false);
        }

        _upgrades[_currentUpgradeIndex].Mesh.SetActive(true);
    }

    public void TryUpgrade()
    {
        CraftingRecipy wantedRecipe = _upgrades[_currentUpgradeIndex].CraftingRecipy;

        BuildingUpgradeUI.Instance.LoadInventoryIngredients();

        if (!CraftingManager.CanCraft(wantedRecipe)) { return; }

        MainInventory.Instance.RemoveItems(wantedRecipe.CraftingIngredients);

        _upgrades[_currentUpgradeIndex].Mesh.SetActive(false);

        _currentUpgradeIndex++;

        _upgrades[_currentUpgradeIndex].Mesh.SetActive(true);

        BuildingUpgradeUI.Instance.EnableUpgradesPanel(false);
    }

    public Upgrade GetCurrentUpgrade()
    {
        return _upgrades[_currentUpgradeIndex];
    }

    [System.Serializable]
    public class Upgrade
    {
        public GameObject Mesh;
        public CraftingRecipy CraftingRecipy;
    }
}
