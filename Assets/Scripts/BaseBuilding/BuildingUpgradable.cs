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

        BuildingUpgradeUI.Instance.Initialize(GetCurrentUpgrade().CraftingRecipy, GetCurrentUpgrade().SpentMaterials);
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
    
    private void AddToSpentMaterials()
    {
        Upgrade wantedUpgrade = _upgrades[_currentUpgradeIndex];

        foreach (ItemQuantity wantedMaterial in wantedUpgrade.CraftingRecipy.CraftingIngredients)
        {
            int inventoryAmount = MainInventory.Instance.GetItemAmount(wantedMaterial.Item);

            if(inventoryAmount == 0) { continue; }

            ItemQuantity spentMaterial = wantedUpgrade.SpentMaterials.Find(x => x.Item);

            if(spentMaterial == null) { wantedUpgrade.SpentMaterials.Add(new ItemQuantity(wantedMaterial.Item, inventoryAmount)); }
            else
            {
                spentMaterial.Amount =  Mathf.Clamp(spentMaterial.Amount + inventoryAmount, 0, wantedMaterial.Amount);
            }
        }
    }

    public void TryUpgrade()
    {
        CraftingRecipy wantedRecipe = _upgrades[_currentUpgradeIndex].CraftingRecipy;

        AddToSpentMaterials();

        BuildingUpgradeUI.Instance.LoadInventoryIngredients();

        if (!CraftingManager.CanCraft(wantedRecipe)) { return; }

        MainInventory.Instance.RemoveItems(wantedRecipe.CraftingIngredients);

        _upgrades[_currentUpgradeIndex].Mesh.SetActive(false);

        _currentUpgradeIndex++;

        _upgrades[_currentUpgradeIndex].Mesh.SetActive(true);

        BuildingUpgradeUI.Instance.EnableUpgradesPanel(false);
    }

    public bool IsUsable()
    {
        return _currentUpgradeIndex > 0;
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
        public List<ItemQuantity> SpentMaterials;
    }
}
