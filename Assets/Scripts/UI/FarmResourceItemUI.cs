using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FarmResourceItemUI : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] protected Image _icon;
    [SerializeField] protected TextMeshProUGUI _itemNameText;
    [SerializeField] protected TextMeshProUGUI _amountText;

    #endregion

    #region Private Variables

    private ItemQuantity _itemQuantity;

    #endregion

    #region Public Properties

    public ItemQuantity ItemQuantity => _itemQuantity;

    #endregion

    public void Initialize(ItemQuantity itemQuantity)
    {
        _itemQuantity = itemQuantity;

        _icon.sprite = itemQuantity.Item.Icon;
        _itemNameText.text = itemQuantity.Item.DisplayName;
        _amountText.text = itemQuantity.Amount.ToString();
    }

}
