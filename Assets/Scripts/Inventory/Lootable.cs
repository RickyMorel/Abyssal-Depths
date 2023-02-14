using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Lootable : MonoBehaviourID
{
    #region Editor Fields

    [SerializeField] private List<ItemQuantity> _loot = new List<ItemQuantity>();
    [SerializeField] private bool _canLoot = false;

    #endregion

    #region Public Properties

    public List<ItemQuantity> LootList => _loot;
    public bool CanLoot { get { return _canLoot; } set { _canLoot = value; } }

    #endregion

    public void TryLoot()
    {
        if (_canLoot == false) { return; }

        Loot(ShipInventory.Instance);
    }

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
        _canLoot = true;
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

        if(this is DeathLoot) { LootUI.Instance.PlayRetrievedItems(); }

        if (this is Minable) { gameObject.SetActive(false); }
        else { Destroy(gameObject); }
    }

    public void SetCanLoot(bool canLoot)
    {
        _canLoot = canLoot;
    }
}
