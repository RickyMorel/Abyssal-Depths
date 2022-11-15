using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minable : Lootable
{
    #region Editor Fields

    //The impact force the pickaxe needs to damage the minable object
    [SerializeField] private float _damageForce;
    [SerializeField] private float _maxHealth;

    #endregion

    #region Private Fields

    private float _currentHealth;

    #endregion

    #region Unity Loops

    private void Start()
    {
        _currentHealth = _maxHealth;
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.O)) { return; }

        Damage(15f);
    }

    #endregion

    public void Damage(float impactForce)
    {
        if(impactForce < _damageForce) { return; }

        float damage = impactForce - _damageForce;

        _currentHealth -= damage;

        CheckIfBreak();
    }

    private void CheckIfBreak()
    {
        if(_currentHealth > 0) { return; }

        ShipInventory shipInventory = FindObjectOfType<ShipInventory>();

        Loot(shipInventory);
    }
}
