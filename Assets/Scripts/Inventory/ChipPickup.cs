using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChipPickup : ItemPickup
{
    #region Editor Fields

    [SerializeField] private TextMeshPro _nameText;

    #endregion

    #region Public Properties

    public PlayerUpgradesController PrevPlayerUpgradesController => _prevPlayerUpgradesController;

    #endregion

    #region Private Variables

    private PlayerUpgradesController _prevPlayerUpgradesController = null;

    #endregion

    #region Unity Loops

    public override void Start()
    {
        base.Start();

        if(ItemSO != null) { Initialize(ItemSO); }
    }

    #endregion

    public override void Initialize(Item item)
    {
        UpgradeChip chip = item as UpgradeChip;
        _itemSO = chip;
        GameObject chipObj = Instantiate(_itemSO.ItemPrefab, transform);
        chipObj.transform.localPosition = Vector3.zero;
        chipObj.transform.localEulerAngles = Vector3.zero;

        _nameText.text = chip.GetChipName();
    }
}