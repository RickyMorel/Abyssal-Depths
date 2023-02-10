using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInventory : Inventory
{
    #region Private Variables

    private static ShipInventory _instance;
    private ShipHealth _shipHealth;

    #endregion

    #region Public Properties

    public static ShipInventory Instance { get { return _instance; } }

    #endregion

    public override void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        base.Awake();
    }

    public override void Start()
    {
        base.Start();

        _shipHealth = GetComponent<ShipHealth>();

        _shipHealth.OnDie += DropAllItems;
    }

    private void OnDestroy()
    {
        _shipHealth.OnDie -= DropAllItems;
    }

    public override void AddItems(List<ItemQuantity> addedItems)
    {
        base.AddItems(addedItems);
        
        LootUI.Instance.DisplayLootedItems(addedItems);
    }

    private void DropAllItems()
    {
        InventoryDictionary.Clear();
    }
}
