using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class BuildingUpgradeUI : MonoBehaviour
{
    #region Editor Fields

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;

    [Header("Panels & Transforms")]
    [SerializeField] private GameObject _buildingUpgradeRequirementsPanel;
    [SerializeField] private Transform _requirementsContentTransform;
    [SerializeField] private BuildingUpgradeIngredientUI _buildingUpgradeItemUIPrefab;

    #endregion

    #region Private Variables

    private static BuildingUpgradeUI _instance;
    private List<BuildingUpgradeIngredientUI> _ingredientUIs = new List<BuildingUpgradeIngredientUI>();

    #endregion

    #region Public Properties

    public static BuildingUpgradeUI Instance { get { return _instance; } }

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

    public void Initialize(CraftingRecipy craftingRecipy, List<ItemQuantity> spentMaterials)
    {
        _itemNameText.text = craftingRecipy.CraftedItem.Item.DisplayName;
        _itemDescriptionText.text = craftingRecipy.CraftedItem.Item.Description;

        DestroyPrevListedItems();

        foreach (ItemQuantity ingredient in craftingRecipy.CraftingIngredients)
        {
            GameObject newIngredientUI = Instantiate(_buildingUpgradeItemUIPrefab.gameObject, _requirementsContentTransform);
            BuildingUpgradeIngredientUI newIngredientScript = newIngredientUI.GetComponent<BuildingUpgradeIngredientUI>();

            ItemQuantity spentItem = spentMaterials.Find(x => x.Item == ingredient.Item);
            int spentAmount = spentItem != null ? spentItem.Amount : 0;

            newIngredientScript.Initialize(ingredient, spentAmount);

            _ingredientUIs.Add(newIngredientScript);
        }

        EnableUpgradesPanel(true);
    }

    public void LoadInventoryIngredients()
    {
        foreach (BuildingUpgradeIngredientUI ingredientUI in _ingredientUIs)
        {
            int inventoryAmount = MainInventory.Instance.GetItemAmount(ingredientUI.ItemQuantity.Item);

            ingredientUI.UpdateSpentAmount(inventoryAmount);
        }
    }

    public void EnableUpgradesPanel(bool isEnabled)
    {
        InventoryUI.Instance.EnableInventory(isEnabled);
        _buildingUpgradeRequirementsPanel.SetActive(isEnabled);
    }

    private void DestroyPrevListedItems()
    {
        _ingredientUIs.Clear();

        foreach (Transform child in _requirementsContentTransform)
        {
            if(child == _requirementsContentTransform) { continue; }

            Destroy(child.gameObject);
        }
    }
}
