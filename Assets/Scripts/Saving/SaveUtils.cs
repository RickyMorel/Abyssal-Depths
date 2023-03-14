using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using static SaveData;

public class SaveUtils
{
    private static UpgradeChip[] _allChips = Resources.LoadAll<UpgradeChip>("ScriptableObjs/Chips");

    //public SaveUtils()
    //{
    //    _allChips = Resources.LoadAll<UpgradeChip>("ScriptableObjs/Chips");
    //}

    public static UpgradeChip GetChipById(string chipId)
    {
        Debug.Log("_allChips: " + _allChips.Length);
        foreach (UpgradeChip chip in _allChips)
        {
            if (chipId != chip.Id) { continue; }

            return chip;
        }

        return null;
    }
}
