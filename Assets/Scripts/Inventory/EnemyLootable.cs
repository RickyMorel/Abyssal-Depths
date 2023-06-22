using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLootable : Lootable
{
    #region Private Variables

    private AIHealth _health;

    #endregion

    private void Start()
    {
        _health = GetComponent<AIHealth>();

        _health.OnDie += HandleDie;
    }

    private void OnDestroy()
    {
        _health.OnDie -= HandleDie;
    }

    private void HandleDie()
    {
        Loot(ShipInventory.Instance);
    }
}
