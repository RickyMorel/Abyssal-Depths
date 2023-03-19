using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/UpgradeChip", order = 2)]
public class UpgradeChip : Item
{
    public ChipType ChipType;
    public int Level = 1;

    public string GetChipName()
    {
        return $"{ChipType.ToString()} Chip MK{Level}";
    }
}

public enum ChipType
{
    None,
    Base,
    Electric,
    Laser,
    Fire
}