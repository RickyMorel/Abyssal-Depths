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
        Debug.Log("DropAllItems");
        GameObject deathLootInstance = Instantiate(GameAssetsManager.Instance.DeathLootPickup, transform.position, Quaternion.identity);
        Lootable lootable = deathLootInstance.GetComponent<Lootable>();
        lootable.AddLoot(ItemDictionaryToList());

        Debug.Log($"deathLootInstance: {deathLootInstance.name}//pos: {deathLootInstance.transform.position}");

        InventoryDictionary.Clear();
    }
}
