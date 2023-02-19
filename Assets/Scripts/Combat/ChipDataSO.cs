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

    public int GetDamageFromChip(BasicChip chipClass, int chipLevel)
    {
        if (chipClass is BaseChip)
        {
            BaseChip baseChip = chipClass as BaseChip;
            switch (chipLevel)
            {
                case 1:
                    return (int)baseChip.MK1Damages[0];
                case 2:
                    return (int)baseChip.MK2Damages[0];
                case 3:
                    return (int)baseChip.MK3Damages[0];
            }
        }
        if (chipClass is FireChip)
        {
            FireChip fireChip = chipClass as FireChip;
            switch (chipLevel)
            {
                case 1:
                    return (int)fireChip.MK1Damages[0];
                case 2:
                    return (int)fireChip.MK2Damages[0];
                case 3:
                    return (int)fireChip.MK3Damages[0];
            }
        }
        if (chipClass is ElectricChip)
        {
            ElectricChip electricChip = chipClass as ElectricChip;
            switch (chipLevel)
            {
                case 1:
                    return (int)electricChip.MK1Damages[0];
                case 2:
                    return (int)electricChip.MK2Damages[0];
                case 3:
                    return (int)electricChip.MK3Damages[0];
            }
        }
        if (chipClass is LaserChip)
        {
            LaserChip laserChip = chipClass as LaserChip;
            switch (chipLevel)
            {
                case 1:
                    return (int)laserChip.MK1Damages[0];
                case 2:
                    return (int)laserChip.MK2Damages[0];
                case 3:
                    return (int)laserChip.MK3Damages[0];
            }
        }
        return -1;
    }
    //cringe
    public float GetSecondaryValueFromChip(BasicChip chipClass, int chipLevel)
    {
        if (chipClass is BaseChip)
        {
            BaseChip baseChip = chipClass as BaseChip;
            switch (chipLevel)
            {
                case 1:
                    return (int)baseChip.MK1Damages[1];
                case 2:
                    return (int)baseChip.MK2Damages[1];
                case 3:
                    return (int)baseChip.MK3Damages[1];
            }
        }
        if (chipClass is FireChip)
        {
            FireChip fireChip = chipClass as FireChip;
            switch (chipLevel)
            {
                case 1:
                    return (int)fireChip.MK1Damages[1];
                case 2:
                    return (int)fireChip.MK2Damages[1];
                case 3:
                    return (int)fireChip.MK3Damages[1];
            }
        }
        if (chipClass is ElectricChip)
        {
            ElectricChip electricChip = chipClass as ElectricChip;
            switch (chipLevel)
            {
                case 1:
                    return (int)electricChip.MK1Damages[1];
                case 2:
                    return (int)electricChip.MK2Damages[1];
                case 3:
                    return (int)electricChip.MK3Damages[1];
            }
        }
        if (chipClass is LaserChip)
        {
            LaserChip laserChip = chipClass as LaserChip;
            switch (chipLevel)
            {
                case 1:
                    return (int)laserChip.MK1Damages[1];
                case 2:
                    return (int)laserChip.MK2Damages[1];
                case 3:
                    return (int)laserChip.MK3Damages[1];
            }
        }
        return -1;
    }

    public BasicChip GetChipType(DamageType damageType)
    {
        if (damageType == DamageType.Base) { return BaseData; }
        else if (damageType == DamageType.Fire) { return FireData; }
        else if (damageType == DamageType.Electric) { return ElectricData; }
        else if (damageType == DamageType.Laser) { return LaserData; }
        return null;
    }

    public void GetWeaknessAndResistance(BasicChip chipClass, out float weakness, out float resistance)
    {
        if (chipClass is BaseChip)
        {
            BaseChip baseChip = chipClass as BaseChip;
            weakness = baseChip.DamageMultiplierWeakness;
            resistance = baseChip.DamageMultiplierResistance;
            return;
        }
        if (chipClass is FireChip)
        {
            FireChip fireChip = chipClass as FireChip;
            weakness = fireChip.DamageMultiplierWeakness;
            resistance = fireChip.DamageMultiplierResistance;
            return;
        }
        if (chipClass is ElectricChip)
        {
            ElectricChip electricChip = chipClass as ElectricChip;
            weakness = electricChip.DamageMultiplierWeakness;
            resistance = electricChip.DamageMultiplierResistance;
            return;
        }
        if (chipClass is LaserChip)
        {
            LaserChip laserChip = chipClass as LaserChip;
            weakness = laserChip.DamageMultiplierWeakness;
            resistance = laserChip.DamageMultiplierResistance;
            return;
        }
        weakness = 0;
        resistance = 0;
        return;
    }

    public float GetImpactDamageFromChip(BasicChip chipClass)
    {
        if (chipClass is FireChip)
        {
            FireChip fireChip = chipClass as FireChip;
            return fireChip.ImpactDamage;
        }
        if (chipClass is ElectricChip)
        {
            ElectricChip electricChip = chipClass as ElectricChip;
            return electricChip.ImpactDamage;
        }
        return -1;
    }

    public float GetAdditionalValueFromChip(BasicChip chipClass)
    {
        if (chipClass is FireChip)
        {
            FireChip fireChip = chipClass as FireChip;
            return fireChip.TimeBetweenBurns;
        }
        if (chipClass is LaserChip)
        {
            LaserChip laserChip = chipClass as LaserChip;
            return laserChip.TimeBetweenHits;
        }
        return -1;
    }

    public float GetBonusFromChip(DamageType damageType)
    {
        return 0;
    }

    #region Helper Classes

    [System.Serializable]
    public class BasicChip
    {
        public float ImpactDamage;
        public float ShootAfterSeconds = 0.2f;
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

    [System.Serializable]
    public class BonusDamages
    {
        [Tooltip("X is for max damage, Y is for time to reach that max damage")]
        public int BaseBonus;
        [Tooltip("X is for max damage, Y is for time to reach that max damage")]
        public int FireBonus;
        [Tooltip("X is for max damage, Y is for time to reach that max damage")]
        public int ElectricBonus;
        [Tooltip("X is for max damage, Y is for time to reach that max damage")]
        public int LaserBonus;
    }

    #endregion
}