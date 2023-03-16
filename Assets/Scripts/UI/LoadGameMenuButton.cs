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

        _playerNameText.text = saveData.ShipName;
        _locationNameText.text = saveData.LocationName;
        _timePlayedText.text = CalculateTimePlayed(saveData.PlayedTime);
        _upgradesText.text = "";

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
