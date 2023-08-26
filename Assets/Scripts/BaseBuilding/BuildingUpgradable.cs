using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingUpgradable : BuildingInteractable
{
    #region Editor Fields

    [SerializeField] protected Upgrade[] _upgrades;
    [SerializeField] protected bool _canUseWithNoUpgrades = false;

    #endregion

    #region Private Variables

    private int _currentUpgradeIndex = 0;

    #endregion

    public override void Start()
    {
        base.Start();

        SetToCurrentUpgrade();
    }

    public override bool Interact()
    {
        if (!CanUse()) { return false; }

        base.Interact();

        return true;
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

    public void ResetUpgrades()
    {
        foreach (Upgrade upgrade in _upgrades)
        {
            upgrade.Mesh.SetActive(false);

            upgrade.SpentMaterials.Clear();
        }

        _currentUpgradeIndex = 0;

        _upgrades[_currentUpgradeIndex].Mesh.SetActive(true);
    }
    
    public static void AddToSpentMaterials(Upgrade wantedUpgrade)
    {
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
        //Cant upgrade if building is broken
        if(BuildingHealth != null && BuildingHealth.IsDead()) { return; }

        //Dont try to upgrade when there are none left
        if (_upgrades.Length - 1 == _currentUpgradeIndex) { return; }

        if (!BuildingUpgradeUI.Instance.IsEnabled()) { BuildingUpgradeUI.Instance.Initialize(GetCurrentUpgrade().CraftingRecipy, GetCurrentUpgrade().SpentMaterials); return; }

        CraftingRecipy wantedRecipe = _upgrades[_currentUpgradeIndex].CraftingRecipy;

        AddToSpentMaterials(_upgrades[_currentUpgradeIndex]);

        BuildingUpgradeUI.Instance.LoadInventoryIngredients();

        if (!CraftingManager.CanCraft(wantedRecipe)) { return; }

        MainInventory.Instance.RemoveItems(wantedRecipe.CraftingIngredients);

        _upgrades[_currentUpgradeIndex].Mesh.SetActive(false);

        _currentUpgradeIndex++;

        _upgrades[_currentUpgradeIndex].Mesh.SetActive(true);

        BuildingUpgradeUI.Instance.EnableUpgradesPanel(false);
    }

    public override bool CanUse()
    {
        if (_canUseWithNoUpgrades)
        {
            return true;
        }
        else
        {
            return _currentUpgradeIndex > 0;
        }
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
