using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingUpgradeIngredientUI : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] protected Image _icon;
    [SerializeField] private Image _progressFillImage;
    [SerializeField] protected TextMeshProUGUI _itemNameText;
    [SerializeField] protected TextMeshProUGUI _amountText;

    #endregion

    #region Private Variables

    private ItemQuantity _itemQuantity;

    #endregion

    #region Public Properties

    public ItemQuantity ItemQuantity => _itemQuantity;

    #endregion

    public void Initialize(ItemQuantity itemQuantity, int spentAmount)
    {
        _itemQuantity = itemQuantity;

        _icon.sprite = itemQuantity.Item.Icon;
        _itemNameText.text = itemQuantity.Item.DisplayName;

        UpdateSpentAmount(spentAmount);
    }

    public void UpdateSpentAmount(int spentAmount)
    {
        _progressFillImage.fillAmount = (float)spentAmount / (float)_itemQuantity.Amount;
        _amountText.text = $"{spentAmount}/{_itemQuantity.Amount}";
    }
}
