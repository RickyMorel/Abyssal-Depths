using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHealth : Damageable
{
    public override void Start()
    {
        base.Start();

        OnUpdateHealth += HandleUpdateHealth;
    }

    public override void Damage(int damage, bool isImpactDamage = false, bool isDamageChain = false, Collider instigatorCollider = null, int index = 0)
    {
        base.Damage(damage, isImpactDamage, isDamageChain, instigatorCollider, index);

        GameStatsPanelUI.Instance.UpdateBaseHealth(CurrentHealth, MaxHealth);
    }

    private void HandleUpdateHealth(int newHealth)
    {
        GameStatsPanelUI.Instance.UpdateBaseHealth(CurrentHealth, MaxHealth);
    }

    public override void Die()
    {
        base.Die();
    }
}
