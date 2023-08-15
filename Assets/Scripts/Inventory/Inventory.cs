using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Inventory : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private Transform _contentTransform;
    [SerializeField] private GameObject _inventoryItemUIPrefab;
    //This is temporary while we don't have an inventory system
    [SerializeField] private List<ItemQuantity> _preloadedItems = new List<ItemQuantity>();
    [SerializeField] private List<ItemQuantity> _dictionaryItemsList = new List<ItemQuantity>();
    //

    #endregion

    #region Private Variables

    private Dictionary<Item, ItemQuantity> _inventory = new Dictionary<Item, ItemQuantity>();
    private Dictionary<string, Item> _itemDatabase = new Dictionary<string, Item>();
    private Chest _currentChest;
    private PlayerInputHandler _currentPlayer;

    #endregion

    #region Public Properties

    public Dictionary<Item, ItemQuantity> InventoryDictionary => _inventory;
    public event Action OnUpdatedInventory;
    public Dictionary<string, Item> ItemDatabase => _itemDatabase;

    #endregion

    #region UI

    public void EnableInventory(bool isEnabled, Chest chest, PlayerInputHandler currentPlayer)
    {
        _inventoryPanel.SetActive(isEnabled);
        _currentChest = chest;
        _currentPlayer = currentPlayer;

        if (isEnabled)
            LoadItems();
    }

    private void LoadItems()
    {
        DestroyItemsUI();

        foreach (KeyValuePair<Item, ItemQuantity> item in _inventory)
        {
            GameObject itemUI = Instantiate(_inventoryItemUIPrefab, _contentTransform);
            itemUI.GetComponent<InventoryItemUI>().Initialize(item.Value, _currentChest, _currentPlayer);
        }
    }

    private void DestroyItemsUI()
    {
        foreach (Transform child in _contentTransform)
        {
            if(child == _contentTransform) { continue; }

            Destroy(child.gameObject);
        }
    }

    #endregion

    public virtual void Awake()
    {
        GetItemDatabase();

        OnUpdatedInventory += HandleUpdateInventory;
    }

    //This is temporary while we don't have an inventory system
    public virtual void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            AddItems(_preloadedItems);
        }
    }
    //

    private void OnDestroy()
    {
        OnUpdatedInventory -= HandleUpdateInventory;
    }

    public List<ItemQuantity> ItemDictionaryToList()
    {
        List<ItemQuantity> listItems = new List<ItemQuantity>();

        foreach (KeyValuePair<Item, ItemQuantity> item in _inventory)
        {
            listItems.Add(item.Value);
        }

        return listItems;
    }

    public void TransferAllItemsToNewInventory(Inventory newInventory)
    {
        List<ItemQuantity> transferedItems = ItemDictionaryToList();

        newInventory.AddItems(transferedItems);
        _inventory.Clear();
    }

    private void GetItemDatabase()
    {
        Item[] allItems = Resources.LoadAll<Item>("ScriptableObjs/Items");

        foreach (Item item in allItems)
        {
            _itemDatabase.Add(item.Id, item);
        }
    }

    public void LoadSavedItems(List<SaveData.ItemData> loadedItems)
    {
        _inventory.Clear();

        List<ItemQuantity> loadedItemsList = new List<ItemQuantity>();

        foreach (SaveData.ItemData item in loadedItems)
        {
            if(!_itemDatabase.TryGetValue(item.Id, out Item wantedItem)) { continue; }

            ItemQuantity itemQuantity = new ItemQuantity(wantedItem, item.Amount);

            loadedItemsList.Add(itemQuantity);
        }

        AddItems(loadedItemsList, false);
    }

    public virtual void AddItems(List<ItemQuantity> addedItems, bool displayAddedItems = true)
    {
        foreach (ItemQuantity itemQuantity in addedItems)
        {
            AddItem(itemQuantity);
        }
    }

    public void AddItem(ItemQuantity itemQuantity)
    {
        if (_inventory.ContainsKey(itemQuantity.Item))
        {
            _inventory[itemQuantity.Item].Amount += itemQuantity.Amount;
        }
        else
        {
            _inventory.Add(itemQuantity.Item, itemQuantity);
        }

        OnUpdatedInventory?.Invoke();
    }

    public void RemoveItems(List<ItemQuantity> removedItems)
    {
        foreach (ItemQuantity itemQuantity in removedItems)
        {
            RemoveItem(itemQuantity);
        }
    }

    public bool HasEnoughItem(ItemQuantity wantedAmount)
    {
        if(!_inventory.TryGetValue(wantedAmount.Item, out ItemQuantity itemQuantity)) { return false; }

        if(itemQuantity.Amount < wantedAmount.Amount) { return false; }

        return true;
    }

    public void RemoveItem(ItemQuantity itemQuantity)
    {
        if (!_inventory.ContainsKey(itemQuantity.Item)) { Debug.LogError("TRYING TO REMOVE ITEM THAT DOESN'T EXIST: " + itemQuantity.Item); return; }

        _inventory[itemQuantity.Item].Amount -= itemQuantity.Amount;

        if (_inventory[itemQuantity.Item].Amount < 1)
            _inventory.Remove(itemQuantity.Item);

        OnUpdatedInventory?.Invoke();

        LoadItems();
    }

    public void RemoveItemsOfType(Item item)
    {
        if (!_inventory.ContainsKey(item)) { Debug.LogError("TRYING TO REMOVE ITEM THAT DOESN'T EXIST: " + item); return; }

        _inventory.Remove(item);

        OnUpdatedInventory?.Invoke();

        LoadItems();
    }

    public int GetItemAmount(Item item)
    {
        if(_inventory.TryGetValue(item, out ItemQuantity itemQuantity)) { return itemQuantity.Amount; }

        return 0;
    }

    private void HandleUpdateInventory()
    {
        _dictionaryItemsList.Clear();

        foreach (KeyValuePair<Item, ItemQuantity> item in _inventory)
        {
            _dictionaryItemsList.Add(item.Value);
        }
    }
}
