using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class CraftingUI : MonoBehaviour
{
    #region Editor Fields

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;

    [Header("Panels & Transforms")]
    [SerializeField] private GameObject _craftingPanel;
    [SerializeField] private Transform _craftablesContentTransform;
    [SerializeField] private Transform _ingredientsContentTransform;
    [SerializeField] private CraftingItemUI _craftableItemUIPrefab;
    [SerializeField] private FarmResourceItemUI _resourceUIPrefab;

    #endregion

    #region Private Variables

    private static CraftingUI _instance;

    #endregion

    #region Public Properties

    public static CraftingUI Instance { get { return _instance; } }

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
    }

    #endregion

    public void Initialize(PlayerInputHandler player)
    {
        DestroyPrevListedItems(_craftablesContentTransform);
        DisplayItemInfo(CraftingManager.Instance.CraftingRecipyList[0]);

        foreach (CraftingRecipy craftable in CraftingManager.Instance.CraftingRecipyList)
        {
            GameObject newCraftableUI = Instantiate(_craftableItemUIPrefab.gameObject, _craftablesContentTransform);
            CraftingItemUI newCraftableUIScript = newCraftableUI.GetComponent<CraftingItemUI>();

            newCraftableUIScript.InitializeItem(craftable, player, null);
        }

        EnablePanel(true);
    }

    public void DisplayItemInfo(CraftingRecipy craftingRecipy)
    {
        _itemNameText.text = craftingRecipy.CraftedItem.Item.GetItemName();
        _itemDescriptionText.text = craftingRecipy.CraftedItem.Item.Description;

        LoadIngredients(craftingRecipy);
    }

    private void LoadIngredients(CraftingRecipy craftingRecipy)
    {
        DestroyPrevListedItems(_ingredientsContentTransform);

        foreach (ItemQuantity resource in craftingRecipy.CraftingIngredients)
        {
            GameObject newResourceUI = Instantiate(_resourceUIPrefab.gameObject, _ingredientsContentTransform);
            FarmResourceItemUI newResourceScript = newResourceUI.GetComponent<FarmResourceItemUI>();

            newResourceScript.Initialize(resource, MainInventory.Instance.GetItemAmount(resource.Item));
        }
    }

    public bool IsEnabled()
    {
        return _craftingPanel.activeSelf;
    }

    public void EnablePanel(bool isEnabled)
    {
        _craftingPanel.SetActive(isEnabled);
    }

    private void DestroyPrevListedItems(Transform contentTransform)
    {
        foreach (Transform child in contentTransform)
        {
            if(child == contentTransform) { continue; }

            Destroy(child.gameObject);
        }
    }
}
