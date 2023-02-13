using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInventory : Inventory
{
    #region Private Variables

    private static ShipInventory _instance;

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

    public override void AddItems(List<ItemQuantity> addedItems)
    {
        base.AddItems(addedItems);
        
        LootUI.Instance.DisplayLootedItems(addedItems);
    }

    public void DropAllItems()
    {
        //Spawn in front of ship
        Vector3 spawnPos = transform.position + Vector3.back * 1.5f;
        GameObject deathLootInstance = Instantiate(GameAssetsManager.Instance.DeathLootPickup, spawnPos, Quaternion.identity);
        Lootable lootable = deathLootInstance.GetComponent<Lootable>();
        lootable.AddLoot(ItemDictionaryToList());

        InventoryDictionary.Clear();
    }
}
