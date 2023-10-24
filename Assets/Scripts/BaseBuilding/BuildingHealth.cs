using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHealth : Damageable
{
    #region Editor Fields

    [SerializeField] private BuildingUpgradable.Upgrade _fixCost;
    [SerializeField] private GameObject[] _objectsToDeactivateWhenDamaged;
    [SerializeField] private GameObject[] _objectsToActivateWhenDamaged;
    [SerializeField] private ParticleSystem _destroyedParticles;

    #endregion

    #region Unity Loops

    public override void Start()
    {
        base.Start();

        OnUpdateHealth += HandleUpdateHealth;
    }

    //Damage will be called by a HitBox object that uses TriggerStayEvent
    public override void OnTriggerStay(Collider other)
    {
        //Do nothing
    }

    public override void OnCollisionStay(Collision collision)
    {
        //Do nothing
    }

    #endregion

    public void TryFix()
    {
        if (!IsDead()) { return; }

        if (!BuildingUpgradeUI.Instance.IsEnabled()) { BuildingUpgradeUI.Instance.Initialize(_fixCost.CraftingRecipy, _fixCost.SpentMaterials); return; }

        BuildingUpgradable.AddToSpentMaterials(_fixCost);

        BuildingUpgradeUI.Instance.LoadInventoryIngredients();

        if (!CraftingManager.CanCraft(_fixCost.CraftingRecipy)) { return; }

        MainInventory.Instance.RemoveItems(_fixCost.CraftingRecipy.CraftingIngredients);

        AddHealth((int)MaxHealth);

        EnableDamagedObjects(false);

        BuildingUpgradeUI.Instance.EnableUpgradesPanel(false);

        if (TryGetComponent(out BuildingUpgradable buildingUpgradable)) { buildingUpgradable.ResetUpgrades(); }
    }

    private void EnableDamagedObjects(bool isDamaged)
    {
        foreach (GameObject obj in _objectsToDeactivateWhenDamaged)
        {
            obj.SetActive(!isDamaged);
        }

        foreach (GameObject obj in _objectsToActivateWhenDamaged)
        {
            obj.SetActive(isDamaged);
        }
    }

    public override void Damage(int damage, bool isImpactDamage = false, bool isDamageChain = false, Collider instigatorCollider = null, int index = 0)
    {
        base.Damage(damage, isImpactDamage, isDamageChain, instigatorCollider, index);
    }

    private void HandleUpdateHealth(int newHealth)
    {
        
    }

    public override void Die()
    {
        base.Die();

        BuildingUpgradeUI.Instance.EnableUpgradesPanel(false);

        EnableDamagedObjects(true);

        _destroyedParticles.Play();
    }
}
