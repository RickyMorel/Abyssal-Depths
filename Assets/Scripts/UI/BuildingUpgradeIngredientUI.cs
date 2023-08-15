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

    public void Initialize(ItemQuantity itemQuantity, int spentAmount)
    {
        _icon.sprite = itemQuantity.Item.Icon;
        _progressFillImage.fillAmount = (float)spentAmount/(float)itemQuantity.Amount;

        _itemNameText.text = itemQuantity.Item.DisplayName;
        _amountText.text = $"{spentAmount}/{itemQuantity.Amount}";
    }
}
