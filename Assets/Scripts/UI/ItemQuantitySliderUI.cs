using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ItemQuantitySliderUI : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private Vector3 _sliderOffset;
    [SerializeField] private GameObject _itemQuantitySliderPanel;
    [SerializeField] private TextMeshProUGUI _currentAmountText;
    [SerializeField] private TextMeshProUGUI _totalAmountText;
    [SerializeField] private Slider _slider;

    #endregion

    #region Private Variables

    private static ItemQuantitySliderUI _instance;
    private int _currentAmount;
    private Chest _currentChest;
    private ItemQuantity _finalItemQuantity = new ItemQuantity();
    private PlayerInputHandler _currentPlayer;

    #endregion

    #region Public Properties
    public static ItemQuantitySliderUI Instance { get { return _instance; } }

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

    public void EnableSlider(bool isEnabled)
    {
        _itemQuantitySliderPanel.SetActive(isEnabled);

        if (isEnabled) { return; }

        if(_currentPlayer == null) { return; }

        _currentPlayer.OnUISubmit -= OnUISubmit;
    }

    public void Initialize(ItemQuantity itemQuantity, Chest chest, PlayerInputHandler currentPlayer, Vector3 cellPosition)
    {
        _itemQuantitySliderPanel.transform.position = cellPosition + _sliderOffset;
        _finalItemQuantity.Item = itemQuantity.Item;
        _currentChest = chest;
        _currentPlayer = currentPlayer;

        _slider.minValue = 0;
        _slider.maxValue = itemQuantity.Amount;
        _currentAmountText.text = _slider.value.ToString();
        _totalAmountText.text = itemQuantity.Amount.ToString();

        _currentPlayer.OnUISubmit += OnUISubmit;

        EnableSlider(true);
    }

    public void HandleSliderMoved()
    {
        if(_finalItemQuantity.Item == null) { return; }

        _currentAmount = (int)_slider.value;
        _currentAmountText.text = _currentAmount.ToString();
        _finalItemQuantity.Amount = _currentAmount;
    }

    private void OnUISubmit()
    {
        if(_itemQuantitySliderPanel.activeSelf == false) { return; }

        _currentChest.SpawnItem(_finalItemQuantity);

        EnableSlider(false);
    }

}
