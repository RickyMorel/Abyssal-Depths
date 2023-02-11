using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootable : MonoBehaviourID
{
    #region Editor Fields

    [SerializeField] private List<ItemQuantity> _loot = new List<ItemQuantity>();
    [SerializeField] private bool _canLoot = false;

    #endregion

    #region Public Properties

    public List<ItemQuantity> LootList => _loot;

    #endregion

    #region Unity Loops

    public virtual void OnTriggerEnter(Collider other)
    {
        if(_canLoot == false) { return; }

        if(_loot.Count < 1) { return; }

        if(!other.gameObject.TryGetComponent<ShipInventory>(out ShipInventory shipInventory)) { return; }

        Loot(shipInventory);
    }

    #endregion

    public void LoadSavedItems(List<SaveData.ItemData> loadedItems)
    {
        _loot.Clear();

        Dictionary<string, Item> itemDatabase = MainInventory.Instance.ItemDatabase;
        List<ItemQuantity> loadedItemsList = new List<ItemQuantity>();

        foreach (SaveData.ItemData item in loadedItems)
        {
            if (!itemDatabase.TryGetValue(item.Id, out Item wantedItem)) { continue; }

            ItemQuantity itemQuantity = new ItemQuantity(wantedItem, item.Amount);

            loadedItemsList.Add(itemQuantity);
        }

        AddLoot(loadedItemsList);
    }

    public void AddLoot(List<ItemQuantity> items)
    {
        foreach (ItemQuantity item in items)
        {
            _loot.Add(item);
        }
    }

    public void Loot(ShipInventory shipInventory)
    {
        shipInventory.AddItems(_loot);

        _loot.Clear();

        if (this is Minable) { gameObject.SetActive(false); }
        else { Destroy(gameObject); }
    }

    public void SetCanLoot(bool canLoot)
    {
        _canLoot = canLoot;
    }
}
