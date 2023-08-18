using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BuildingHealth))]
public class BaseCore : BuildingInteractable
{
    #region Private Variables

    private BuildingHealth _health;

    #endregion

    public override void Start()
    {
        base.Start();

        _health = GetComponent<BuildingHealth>();

        _health.OnDie += HandleDie;
    }

    public override void Interact()
    {
        base.Interact();

        _health.AddHealth(200);

        GameStatsPanelUI.Instance.UpdateBaseHealth(_health.CurrentHealth, _health.MaxHealth);
    }

    private void HandleDie()
    {
        GameManager.Instance.GameOver();
    }
}
