using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Analytics;

public class CraftingManager : MonoBehaviour
{
    #region Editor Fields

    [Header("Description Panel")]
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;
    [SerializeField] private Transform _ingredientsContentTransform;

    [Header("Items Panel")]
    [SerializeField] private GameObject _craftingPanel;
    [SerializeField] private Transform _contentTransform;
    [SerializeField] private GameObject _craftingItemUIPrefab;
    [SerializeField] private List<CraftingRecipy> _craftingRecipyList = new List<CraftingRecipy>();

    [Header("Main Inventory Panel")]
    [SerializeField] private Transform _inventoryContentTransform;

    #endregion

    #region Private Variables

    private static GameObject _itemUIPrefab;
    private PlayerInputHandler _currentPlayer;
    private CraftingStation _currentCraftingStation;
    private static CraftingManager _instance;

    #endregion

    #region Public Properties

    public static CraftingManager Instance { get { return _instance; } }

    #endregion

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        _itemUIPrefab = (GameObject)Resources.Load("ItemUIButton");
        //Will uncomment later
        //_craftingRecipyList = Resources.LoadAll<CraftingRecipy>("ScriptableObjs/CraftingRecipies").ToList();
    }

    public static bool CanCraft(CraftingRecipy craftingRecipy)
    {
        Dictionary<Item, ItemQuantity> ownedItems = MainInventory.Instance.InventoryDictionary;
        foreach (ItemQuantity itemQuantity in craftingRecipy.CraftingIngredients)
        {
            //if has item
            if (!ownedItems.TryGetValue(itemQuantity.Item, out ItemQuantity ownedItemQuantity)) { return false; }

            //if has correct amount
            if (ownedItemQuantity.Amount < itemQuantity.Amount) { return false; }
        }

        return true;
    }

    public void DisplayItemInfo(CraftingRecipy craftingRecipy)
    {
        _itemNameText.text = craftingRecipy.CraftedItem.Item.GetItemName();
        _itemDescriptionText.text = craftingRecipy.CraftedItem.Item.Description;

        LoadIngredients(craftingRecipy, _ingredientsContentTransform);
    }

    public void EnableCanvas(bool isEnabled, PlayerInputHandler currentPlayer, CraftingStation craftingStation)
    {
        _craftingPanel.SetActive(isEnabled);
        _currentPlayer = currentPlayer;
        _currentCraftingStation = craftingStation;

        DestroyItemsUI(_ingredientsContentTransform);

        if (isEnabled)
        {
            LoadItems();
            LoadInventory();
        }
    }

    public static void LoadIngredients(CraftingRecipy craftingRecipy, Transform contentTransform)
    {
        DestroyItemsUI(contentTransform);

        foreach (ItemQuantity ingredient in craftingRecipy.CraftingIngredients)
        {
            GameObject itemUI = Instantiate(_itemUIPrefab, contentTransform);
            itemUI.GetComponent<ItemUI>().Initialize(ingredient, null);
        }
    }

    public void LoadInventory()
    {
        DestroyItemsUI(_inventoryContentTransform);

        List<ItemQuantity> listNumber = new List<ItemQuantity>(MainInventory.Instance.InventoryDictionary.Values);

        foreach (ItemQuantity material in listNumber)
        {
            GameObject itemUI = Instantiate(_itemUIPrefab, _inventoryContentTransform);
            itemUI.GetComponent<ItemUI>().Initialize(material, null);
        }
    }

    private void LoadItems()
    {
        DestroyItemsUI(_contentTransform);

        foreach (CraftingRecipy craftable in _craftingRecipyList)
        {
            if (craftable.IsRepair) { continue; }
            GameObject itemUI = Instantiate(_craftingItemUIPrefab, _contentTransform);
            itemUI.GetComponent<CraftingItemUI>().Initialize(craftable, _currentPlayer, _currentCraftingStation);
        }
    }

    public static void DestroyItemsUI(Transform contentTransform)
    {
        foreach (Transform child in contentTransform)
        {
            if (child == contentTransform) { continue; }

            Destroy(child.gameObject);
        }
    }
}
