using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Damageable
{
    #region Editor Fields

    [Header("FX")]
    [SerializeField] private ParticleSystem _hitParticles;

    #endregion

    #region Private Variables

    private Lootable _lootableScript;

    #endregion

    #region Unity Loops

    public override void Start()
    {
        base.Start();

        _lootableScript = GetComponent<Lootable>();

        OnDamaged += HandleDamaged;
    }

    private void OnDestroy()
    {
        OnDamaged -= HandleDamaged;
    }

    #endregion

    private void HandleDamaged(DamageTypes damageType)
    {
        _hitParticles.Play();
    }

    public override void Die()
    {
        base.Die();

        _lootableScript.SetCanLoot(true);
    }
}
