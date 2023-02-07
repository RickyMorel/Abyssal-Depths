using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Combat/Chip")]

public class ChipDataSO : ScriptableObject
{
    public List<ChipData> ChipData = new List<ChipData>();
}
[System.Serializable]
public class ChipData
{
    public ChipType Chiptype;
    public int ChipLevel;
    public float ShootAfterSeconds;
    public int Damage;
    public int DamageMultiplierWeakness;
    public int DamageMultiplierResistance;
}