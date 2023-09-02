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

    protected ItemQuantity _itemQuantity;

    #endregion

    #region Public Properties

    public ItemQuantity ItemQuantity => _itemQuantity;

    #endregion

    public virtual void Initialize(ItemQuantity itemQuantity, int amountInInventory = -1)
    {
        _itemQuantity = itemQuantity;

        _icon.sprite = itemQuantity.Item.Icon;
        _itemNameText.text = itemQuantity.Item.DisplayName;
        _amountText.text = itemQuantity.Amount.ToString();

        if(amountInInventory == -1) { return; }

        _amountText.text = $"{amountInInventory}/{itemQuantity.Amount.ToString()}";

        int slashIndex = _amountText.text.IndexOf("/");

        // Replace text with color value for character.
        _amountText.text = _amountText.text.Replace(_amountText.text[slashIndex].ToString(), "<color=#000000>" + _amountText.text[slashIndex].ToString() + "</color>");

        string greenHexValue = "159600";
        string redHexValue = "FF0000";
        string wantedColor = MainInventory.Instance.GetItemAmount(itemQuantity.Item) >= itemQuantity.Amount ? greenHexValue : redHexValue;

        for (int i = 0; i < _amountText.text.Length; i++)
        {
            if(i < slashIndex)
            {
                //_amountText.text = _amountText.text.Replace(_amountText.text[i].ToString(), $"<color=#{wantedColor}>" + _amountText.text[i].ToString() + "</color>");
            }
        }
    }

}
