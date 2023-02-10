using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Combat/Chip")]

public class ChipDataSO : ScriptableObject
{
    public BaseChip BaseData;
    public FireChip FireData;
    public ElectricChip ElectricData;
    public LaserChip LaserData;
}

[System.Serializable]
public class BaseChip
{
    public float ShootAfterSeconds;
    public int MinimumDamage;
    public int MaximumDamage;
    public int MinimumDamageMK2;
    public int MaximumDamageMK2;
    public int MinimumDamageMK3;
    public int MaximumDamageMK3;
    public int DamageMultiplierWeakness;
    public int DamageMultiplierResistance;
}

[System.Serializable]
public class FireChip
{
    public float ShootAfterSeconds;
    public float HitDamage;
    public int AfterburnDamage;
    public int AfterburnDamageMK2;
    public int AfterburnDamageMK3;
    public float BurnAfterSeconds;
    public float BurnDuration;
    public int DamageMultiplierWeakness;
    public int DamageMultiplierResistance;
}

[System.Serializable]
public class ElectricChip
{
    public float ShootAfterSeconds;
    public float HitDamage;
    public int AfterburnDamage;
    public int AfterburnDamageMK2;
    public int AfterburnDamageMK3;
    public int DamageMultiplierWeakness;
    public int DamageMultiplierResistance;
}

[System.Serializable]
public class LaserChip
{
    public float ShootAfterSeconds;
    public float HitDamage;
    public int AfterburnDamage;
    public int AfterburnDamageMK2;
    public int AfterburnDamageMK3;
    public int DamageMultiplierWeakness;
    public int DamageMultiplierResistance;
}