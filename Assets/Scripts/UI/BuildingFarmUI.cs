using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class BuildingFarmUI : MonoBehaviour
{
    #region Editor Fields

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;

    [Header("Panels & Transforms")]
    [SerializeField] private GameObject _buildingFarmPanel;
    [SerializeField] private Transform _resourcesContentTransform;
    [SerializeField] private FarmResourceItemUI _buildingFarmResourceUIPrefab;

    #endregion

    #region Private Variables

    private static BuildingFarmUI _instance;
    private List<FarmResourceItemUI> _resourceUIs = new List<FarmResourceItemUI>();

    #endregion

    #region Public Properties

    public static BuildingFarmUI Instance { get { return _instance; } }

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
            GameObject newIngredientUI = Instantiate(_buildingFarmResourceUIPrefab.gameObject, _resourcesContentTransform);
            FarmResourceItemUI newResourceScript = newIngredientUI.GetComponent<FarmResourceItemUI>();

            ItemQuantity spentItem = spentMaterials.Find(x => x.Item == ingredient.Item);
            int spentAmount = spentItem != null ? spentItem.Amount : 0;

            newResourceScript.Initialize(ingredient);

            _resourceUIs.Add(newResourceScript);
        }

        EnablePanel(true);
    }

    public void EnablePanel(bool isEnabled)
    {
        _buildingFarmPanel.SetActive(isEnabled);
    }

    private void DestroyPrevListedItems()
    {
        _resourceUIs.Clear();

        foreach (Transform child in _resourcesContentTransform)
        {
            if(child == _resourcesContentTransform) { continue; }

            Destroy(child.gameObject);
        }
    }
}
