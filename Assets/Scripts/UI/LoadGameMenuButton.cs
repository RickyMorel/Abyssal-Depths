using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameMenuButton : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private TextMeshProUGUI _locationNameText;
    [SerializeField] private TextMeshProUGUI _timePlayedText;
    [SerializeField] private TextMeshProUGUI _upgradesText;

    public LoadGameMenuButton()
    {
        _upgradesText.text = "";
    }

    public void DisplayData(SaveData saveData)
    {
        _upgradesText.text += "Wpns(";
        foreach (var upgradeData in saveData.WeaponDatas)
        {
            UpgradeChip chip_1 = SaveUtils.GetChipById(upgradeData.Socket1ChipId);
            UpgradeChip chip_2 = SaveUtils.GetChipById(upgradeData.Socket2ChipId);

            if(chip_1 != null) { _upgradesText.text += $"{chip_1.GetChipName()}, "; }
            if(chip_2 != null) { _upgradesText.text += $"{chip_2.GetChipName()}"; }
        }
        _upgradesText.text += "), ";
    }
}
