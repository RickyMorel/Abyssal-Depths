using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Combat/ChipData")]

public class ChipDataSO : ScriptableObject
{
    public BaseChip BaseData;
    public FireChip FireData;
    public ElectricChip ElectricData;
    public LaserChip LaserData;

    public int GetDamageFromChip(BasicChip chipClass)
    {
        if (chipClass is BaseChip)
        {
            BaseChip baseChip = chipClass as BaseChip;
            return (int)baseChip.MK1Damages[0];
        }
        if (chipClass is FireChip)
        {
            FireChip fireChip = chipClass as FireChip;
            return (int)fireChip.MK1Damages[0];
        }
        if (chipClass is ElectricChip)
        {
            ElectricChip electricChip = chipClass as ElectricChip;
            return (int)electricChip.MK1Damages[0];
        }
        if (chipClass is LaserChip)
        {
            LaserChip laserChip = chipClass as LaserChip;
            return (int)laserChip.MK1Damages[0];
        }
        return -1;
    }

    public float GetSecondaryValueFromChip(BasicChip chipClass)
    {
        if (chipClass is BaseChip)
        {
            BaseChip baseChip = chipClass as BaseChip;
            return (int)baseChip.MK1Damages[1];
        }
        if (chipClass is FireChip)
        {
            FireChip fireChip = chipClass as FireChip;
            return (int)fireChip.MK1Damages[1];
        }
        if (chipClass is ElectricChip)
        {
            ElectricChip electricChip = chipClass as ElectricChip;
            return (int)electricChip.MK1Damages[1];
        }
        if (chipClass is LaserChip)
        {
            LaserChip laserChip = chipClass as LaserChip;
            return (int)laserChip.MK1Damages[1];
        }
        return -1;
    }

    public BasicChip GetChipType(BasicChip chipClass, DamageType damageType)
    {
        if (damageType == DamageType.Base) { return BaseData; }
        else if (damageType == DamageType.Fire) { return FireData; }
        else if (damageType == DamageType.Electric) { return ElectricData; }
        else if (damageType == DamageType.Laser) { return LaserData; }
        return null;
    }

    #region Helper Classes

    [System.Serializable]
    public class BasicChip
    {
        public float ShootAfterSeconds;
        public float DamageMultiplierWeakness;
        public float DamageMultiplierResistance;
    }

    [System.Serializable]
    public class BaseChip : BasicChip
    {
        [Tooltip("X is minimum damage, Y is maximum damage")]
        public Vector2 MK1Damages;
        [Tooltip("X is minimum damage, Y is maximum damage")]
        public Vector2 MK2Damages;
        [Tooltip("X is minimum damage, Y is maximum damage")]
        public Vector2 MK3Damages;


    }

    [System.Serializable]
    public class FireChip : BasicChip
    {
        public float ImpactDamage;
        [Tooltip("X is for fire damage, Y is for burn time")]
        public Vector2 MK1Damages;
        [Tooltip("X is for fire damage, Y is for burn time")]
        public Vector2 MK2Damages;
        [Tooltip("X is for fire damage, Y is for burn time")]
        public Vector2 MK3Damages;
        public float TimeBetweenBurns;
    }

    [System.Serializable]
    public class ElectricChip : BasicChip
    {
        public float ImpactDamage;
        [Tooltip("X is for electric damage, Y is for paralysis time")]
        public Vector2 MK1Damages;
        [Tooltip("X is for electric damage, Y is for paralysis time")]
        public Vector2 MK2Damages;
        [Tooltip("X is for electric damage, Y is for paralysis time")]
        public Vector2 MK3Damages;
    }

    [System.Serializable]
    public class LaserChip : BasicChip
    {
        public float TimeBetweenHits;
        [Tooltip("X is for max damage, Y is for time to reach that max damage")]
        public Vector2 MK1Damages;
        [Tooltip("X is for max damage, Y is for time to reach that max damage")]
        public Vector2 MK2Damages;
        [Tooltip("X is for max damage, Y is for time to reach that max damage")]
        public Vector2 MK3Damages;
    }

    #endregion
}