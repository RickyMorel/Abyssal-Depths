using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGameMenuButton : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private TextMeshProUGUI _locationNameText;
    [SerializeField] private TextMeshProUGUI _timePlayedText;
    [SerializeField] private TextMeshProUGUI _upgradesText;

    #endregion

    #region Private Variables

    private int _saveIndex;

    #endregion

    public void DisplayData(SaveData saveData, int saveIndex)
    {
        _saveIndex = saveIndex;

        _icon.sprite = SaveUtils.ConvertBytesToTexture2D(saveData.ScreenShotBytes);
        _playerNameText.text = saveData.ShipName;
        _locationNameText.text = saveData.LocationName;
        _timePlayedText.text = CalculateTimePlayed(saveData.PlayedTime);
        _upgradesText.text = "Upgrades: ";

        DisplayBoosterData(saveData);
        DisplayWeaponData(saveData);
        DisplayShieldData(saveData);
    }

    private void DisplayBoosterData(SaveData saveData)
    {
        _upgradesText.text += "Bstr(";

        WeaponSO booster = GetUpgradableSO(saveData.BoosterData, UpgradableType.Booster);

        if (booster != null) { _upgradesText.text += $"{booster.WeaponName}"; }

        _upgradesText.text += "), ";
    }

    private void DisplayShieldData(SaveData saveData)
    {
        _upgradesText.text += "Shld(";

        WeaponSO shield = null;

        if (shield != null) { _upgradesText.text += $"{shield.WeaponName}"; }
        else { _upgradesText.text += $"?"; }

        _upgradesText.text += ")";
    }

    private void DisplayWeaponData(SaveData saveData)
    {
        _upgradesText.text += "Wpns(";
        foreach (var upgradeData in saveData.WeaponDatas)
        {
            WeaponSO weapon = GetUpgradableSO(upgradeData, UpgradableType.Weapon);

            if (weapon != null) { _upgradesText.text += $"{weapon.WeaponName}, "; }
        }
        _upgradesText.text += "), ";
    }

    private static WeaponSO GetUpgradableSO(SaveData.UpgradableData upgradeData, UpgradableType upgradableType)
    {
        UpgradeChip chip_1 = SaveUtils.GetChipById(upgradeData.Socket1ChipId);
        UpgradeChip chip_2 = SaveUtils.GetChipById(upgradeData.Socket2ChipId);
        ChipType chip_1_type = chip_1 != null ? chip_1.ChipType : ChipType.None;
        ChipType chip_2_type = chip_2 != null ? chip_2.ChipType : ChipType.None;

        WeaponSO weapon = SaveUtils.GetUpgradableByChips(chip_1_type, chip_2_type, upgradableType);
        return weapon;
    }

    private string CalculateTimePlayed(float totalSecondsPlayed)
    {
        int totalMinutes = Mathf.FloorToInt(totalSecondsPlayed / 60);
        int hours = Mathf.FloorToInt(totalSecondsPlayed / 3600);
        int minutes = totalMinutes - (hours * 60);
        int seconds = Mathf.FloorToInt(totalSecondsPlayed - (totalMinutes * 60));

        return $"{PadNumber(hours)}:{PadNumber(minutes)}:{PadNumber(seconds)}";
    }

    public void LoadGame()
    {
        GameObject saveDataObj = new GameObject($"Loaded Data:{_saveIndex}");
        DontDestroyOnLoad(saveDataObj);

        SceneManager.LoadScene(1);
    }

    private string PadNumber(float number)
    {
        string numberString = number.ToString();
        return numberString.PadLeft(2, '0');
    }
}
