using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private Transform _inventoryContentTransform;

    #endregion

    #region Private Variables

    private static InventoryUI _instance;

    private static GameObject _itemUIPrefab;

    #endregion

    #region Public Properties

    public static InventoryUI Instance { get { return _instance; } }

    #endregion

    #region Unity Loops

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
    }

    #endregion

    public void EnableInventory(bool isEnabled)
    {
        _inventoryPanel.SetActive(isEnabled);

        if (isEnabled)
        {
            LoadInventory();
        }
    }

    public void LoadInventory()
    {
        DestroyItemsUI(_inventoryContentTransform);

        foreach (ItemQuantity material in MainInventory.Instance.InventoryDictionary.Values)
        {
            GameObject itemUI = Instantiate(_itemUIPrefab, _inventoryContentTransform);
            itemUI.GetComponent<ItemUI>().Initialize(material, null);
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
