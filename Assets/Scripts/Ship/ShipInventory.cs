using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShipInventory : Inventory
{
    #region Private Variables

    private static ShipInventory _instance;

    #endregion

    #region Public Properties

    public static ShipInventory Instance { get { return _instance; } }

    #endregion

    #region Unity Loops

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

    #endregion

    public override void AddItems(List<ItemQuantity> addedItems)
    {
        base.AddItems(addedItems);
        
        LootUI.Instance.DisplayLootedItems(addedItems);
    }

    public void DropAllItems()
    {
        //Spawn in front of ship
        Vector3 spawnPos = transform.position + Vector3.back * 2.5f;
        GameObject deathLootInstance = Instantiate(GameAssetsManager.Instance.DeathLootPickup, spawnPos, Quaternion.identity);
        Lootable lootable = deathLootInstance.GetComponent<Lootable>();
        lootable.AddLoot(ItemDictionaryToList());

        InventoryDictionary.Clear();
    }

    public void ShipTryLoot(Collider other)
    {
        if (!other.gameObject.TryGetComponent<Lootable>(out Lootable loot)) { return; }

        loot.TryLoot();
    }
}
