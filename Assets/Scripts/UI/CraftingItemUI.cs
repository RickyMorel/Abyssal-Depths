using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class CraftingItemUI : ItemUI, IPointerEnterHandler
{
    #region Private Variables

    private CraftingRecipy _craftingRecipy;
    private CraftingStation _currentCraftingStation;

    #endregion

    #region Public Properties

    public CraftingRecipy CraftingRecipy => _craftingRecipy;

    #endregion

    private void Start()
    {
        CraftingStation.OnCraft += HandleItemCrafted;
    }

    private void OnDestroy()
    {
        CraftingStation.OnCraft -= HandleItemCrafted;
    }

    private void HandleItemCrafted()
    {
        if(_craftingRecipy == null || _currentCraftingStation == null) { return; }

        if (CraftingManager.CanCraft(_craftingRecipy)) SetGreyScale(0); else SetGreyScale(1);
    }

    public override void Initialize(CraftingRecipy craftingRecipy, PlayerInputHandler currentPlayer, CraftingStation craftingStation)
    {
        _itemQuantity = craftingRecipy.CraftedItem;

        _icon.sprite = _itemQuantity.Item.Icon;
        _amountText.text = $"x{_itemQuantity.Amount}";

        _currentPlayer = currentPlayer;
        _currentCraftingStation = craftingStation;
        _craftingRecipy = craftingRecipy;

        if (CraftingManager.CanCraft(craftingRecipy)) SetGreyScale(0); else SetGreyScale(1);
    }


    public override void OnClick()
    {
        if (!_gotClicked) { return; }

        _gotClicked = false;

        _currentCraftingStation.TryCraft(CraftingRecipy);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CraftingManager.Instance.DisplayItemInfo(CraftingRecipy);
    }
}
