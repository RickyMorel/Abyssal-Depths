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

    public int GetDamageFromChip(BasicChip chipClass, int chipLevel, int selectedDamage)
    {
        if (chipClass is BaseChip)
        {
            BaseChip baseChip = chipClass as BaseChip;
            switch (chipLevel)
            {
                case 1:
                    return (int)baseChip.MK1Damages[selectedDamage];
                case 2:
                    return (int)baseChip.MK2Damages[selectedDamage];
                case 3:
                    return (int)baseChip.MK3Damages[selectedDamage];
            }
        }
        if (chipClass is FireChip)
        {
            FireChip fireChip = chipClass as FireChip;
            switch (chipLevel)
            {
                case 1:
                    return (int)fireChip.MK1Damages[selectedDamage];
                case 2:
                    return (int)fireChip.MK2Damages[selectedDamage];
                case 3:
                    return (int)fireChip.MK3Damages[selectedDamage];
            }
        }
        if (chipClass is ElectricChip)
        {
            ElectricChip electricChip = chipClass as ElectricChip;
            switch (chipLevel)
            {
                case 1:
                    return (int)electricChip.MK1Damages[selectedDamage];
                case 2:
                    return (int)electricChip.MK2Damages[selectedDamage];
                case 3:
                    return (int)electricChip.MK3Damages[selectedDamage];
            }
        }
        if (chipClass is LaserChip)
        {
            LaserChip laserChip = chipClass as LaserChip;
            switch (chipLevel)
            {
                case 1:
                    return (int)laserChip.MK1Damages[selectedDamage];
                case 2:
                    return (int)laserChip.MK2Damages[selectedDamage];
                case 3:
                    return (int)laserChip.MK3Damages[selectedDamage];
            }
        }
        return 20;
    }

    public BasicChip GetChipType(DamageType damageType)
    {
        if (damageType == DamageType.Base) { return BaseData; }
        else if (damageType == DamageType.Fire) { return FireData; }
        else if (damageType == DamageType.Electric) { return ElectricData; }
        else if (damageType == DamageType.Laser) { return LaserData; }
        return null;
    }

    public int GetImpactDamageFromChip(BasicChip chipClass)
    {
        int impactDamage;
        if (chipClass is BaseChip)
        {
            BaseChip baseChip = chipClass as BaseChip;
            impactDamage = baseChip.ImpactDamage;
            return impactDamage;
        }
        if (chipClass is FireChip)
        {
            FireChip fireChip = chipClass as FireChip;
            impactDamage = fireChip.ImpactDamage;
            return impactDamage;
        }
        if (chipClass is ElectricChip)
        {
            ElectricChip electricChip = chipClass as ElectricChip;
            impactDamage = electricChip.ImpactDamage;
            return impactDamage;
        }
        if (chipClass is LaserChip)
        {
            LaserChip laserChip = chipClass as LaserChip;
            impactDamage = laserChip.ImpactDamage;
            return impactDamage;
        }
        return -1;
    }

    //public void GetWeaknessAndResistance(BasicChip chipClass, out float weakness, out float resistance)
    //{
    //    if (chipClass is BaseChip)
    //    {
    //        BaseChip baseChip = chipClass as BaseChip;
    //        baseChip.DamageMultipliers = _damageMultipliersValues.
    //        return;
    //    }
    //    if (chipClass is FireChip)
    //    {
    //        FireChip fireChip = chipClass as FireChip;
    //        weakness = fireChip.DamageMultiplierWeakness;
    //        resistance = fireChip.DamageMultiplierResistance;
    //        return;
    //    }
    //    if (chipClass is ElectricChip)
    //    {
    //        ElectricChip electricChip = chipClass as ElectricChip;
    //        weakness = electricChip.DamageMultiplierWeakness;
    //        resistance = electricChip.DamageMultiplierResistance;
    //        return;
    //    }
    //    if (chipClass is LaserChip)
    //    {
    //        LaserChip laserChip = chipClass as LaserChip;
    //        weakness = laserChip.DamageMultiplierWeakness;
    //        resistance = laserChip.DamageMultiplierResistance;
    //        return;
    //    }
    //    weakness = 0;
    //    resistance = 0;
    //    return;
    //}

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
        if (chipClass is ElectricChip)
        {
            ElectricChip electricChip = chipClass as ElectricChip;
            return electricChip.StunRadius;
        }
        return -1;
    }

    public void GetBonusFromChip(BasicChip chipClass, ref int damage, ref float secondaryValue, ref float additionalValue)
    {
        if (chipClass is BaseChip)
        {
            damage = (int)(damage * 1.5f);
            return;
        }
        if (chipClass is FireChip)
        {
            damage = damage * 2;
            additionalValue = additionalValue / 2;
            return;
        }
        if (chipClass is ElectricChip)
        {
            additionalValue = additionalValue * 1.1f;
            return;
        }
        if (chipClass is LaserChip)
        {
            secondaryValue = secondaryValue * 0.5f;
            return;
        }
        return;
    }

    #region Helper Classes

    [System.Serializable]
    public class BasicChip
    {
        public int ImpactDamage;
        public float ShootAfterSeconds = 0.2f;
        private Vector2 _damageMultipliers;
        public Vector2 DamageMultipliers { get { return _damageMultipliers; } set { _damageMultipliers = value; } }
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
        public float StunRadius;
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