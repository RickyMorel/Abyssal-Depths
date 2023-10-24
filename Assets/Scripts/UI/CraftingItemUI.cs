using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class CraftingItemUI : FarmResourceItemUI, IPointerEnterHandler
{
    #region Editor Fields

    [SerializeField] protected TextMeshProUGUI _damageText;
    [SerializeField] protected Image _damageIcon;

    #endregion

    #region Private Variables

    private CraftingRecipy _craftingRecipy;
    private CraftingStation _currentCraftingStation;
    private PlayerInputHandler _currentPlayer;
    private bool _gotClicked = false;

    #endregion

    #region Public Properties

    public CraftingRecipy CraftingRecipy => _craftingRecipy;

    #endregion

    #region Unity Loops

    private void Awake()
    {
        PlayerInputHandler.OnClick += HandleClick;
    }

    private void OnDestroy()
    {
        PlayerInputHandler.OnClick -= HandleClick;
    }

    #endregion

    public void InitializeItem(CraftingRecipy craftingRecipy, PlayerInputHandler currentPlayer, CraftingStation craftingStation)
    {
        Debug.Log("Craft Initialize:" + craftingRecipy.CraftedItem.Item.DisplayName + " " + craftingRecipy.CraftedItem.Amount);
        _itemQuantity = craftingRecipy.CraftedItem;
        _currentPlayer = currentPlayer;

        _icon.sprite = _itemQuantity.Item.Icon;
        _itemNameText.text = _itemQuantity.Item.DisplayName;
        _amountText.text = $"x{MainInventory.Instance.GetItemAmount(_itemQuantity.Item)} In Inventory";

        if(_itemQuantity.Item is not UpgradeChip) { _damageText.enabled = false;  _damageIcon.enabled = false; }

        _currentCraftingStation = craftingStation;
        _craftingRecipy = craftingRecipy;
    }

    private void HandleClick(PlayerInputHandler playerThatClicked)
    {
        if (playerThatClicked != _currentPlayer) { return; }

        _gotClicked = true;
    }

    public void OnClick()
    {
        if (!_gotClicked) { return; }

        _gotClicked = false;

        bool didCraft = CraftingManager.Instance.TryCraftAndAddToInventory(CraftingRecipy);

        //Refresh UI
        if (didCraft) { CraftingUI.Instance.Initialize(_currentPlayer); }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CraftingUI.Instance.DisplayItemInfo(CraftingRecipy);
    }
}
