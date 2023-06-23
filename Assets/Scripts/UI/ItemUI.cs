using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ItemUI : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] protected Image _icon;
    [SerializeField] protected TextMeshProUGUI _amountText;

    #endregion

    #region Private Variables

    protected ItemQuantity _itemQuantity;
    protected PlayerInputHandler _currentPlayer;
    protected bool _gotClicked = false;
    private ItemQuantity _itemInInventory;

    #endregion

    private void Awake()
    {
        PlayerInputHandler.OnClick += HandleClick;
    }

    private void OnDestroy()
    {
        PlayerInputHandler.OnClick -= HandleClick;
    }

    public virtual void Initialize(ItemQuantity itemQuantity, PlayerInputHandler currentPlayer)
    {
        _icon.sprite = itemQuantity.Item.Icon;
        _amountText.text = $"x{itemQuantity.Amount}";
        SetGreyScale(0);

        _itemQuantity = itemQuantity;

        _currentPlayer = currentPlayer;

        if (MainInventory.Instance.HasEnoughItem(itemQuantity)) { return; }

        _amountText.color = Color.red;
        SetGreyScale(1);
    }

    //This is for the ingredients that appear while crafting
    public void Initialize(ItemQuantity itemQuantity)
    {
        _icon.sprite = itemQuantity.Item.Icon;
        _itemInInventory = null;

        foreach (ItemQuantity item in MainInventory.Instance.InventoryDictionary.Values)
        {
            if (itemQuantity.Item == item.Item) { _itemInInventory = item; break; }
        }

        if (_itemInInventory == null) { _amountText.text = $"{itemQuantity.Amount}/0"; }
        else { _amountText.text = $"{itemQuantity.Amount}/{_itemInInventory.Amount}"; }

        SetGreyScale(0);

        _itemQuantity = itemQuantity;

        if (MainInventory.Instance.HasEnoughItem(itemQuantity)) { return; }

        _amountText.color = Color.red;
        SetGreyScale(1);
    }

    public virtual void Initialize(ItemQuantity itemQuantity, Chest chest, PlayerInputHandler currentPlayer) { }
    public virtual void Initialize(CraftingRecipy craftingRecipy, PlayerInputHandler currentPlayer, CraftingStation craftingStation) { }

    private void HandleClick(PlayerInputHandler playerThatClicked)
    {
        if (playerThatClicked != _currentPlayer) { return; }

        _gotClicked = true;
    }

    public void SetGreyScale(float amount)
    {
        Material materialInstance = Instantiate(_icon.material);
        materialInstance.SetFloat("_GrayscaleAmount", amount);
        _icon.material = materialInstance;
    }

    public virtual void OnClick()
    {
        if (!_gotClicked) { return; }

        _gotClicked = false;
    }
}
