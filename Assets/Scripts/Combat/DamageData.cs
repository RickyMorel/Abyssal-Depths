public class DamageData
{
    #region Private Variables

    protected DamageTypes[] _damageTypes;
    protected int[] _damage = { 0, 0 };
    protected int _impactDamage;
    protected float[] _weakness = { 0, 0 };
    protected float[] _resistance = { 0, 0 };
    protected float[] _secondaryValue = { 0, 0 };
    protected float[] _additionalValue = { 0, 0 };
    protected int _chipLevel;

    #endregion

    #region Public Properties

    public int[] Damage => _damage;
    public int ImpactDamage => _impactDamage;
    public DamageTypes[] DamageTypes => _damageTypes;
    public float[] Weakness => _weakness;
    public float[] Resistance => _resistance;
    public float[] SecondaryValue => _secondaryValue;
    public float[] AdditionalValue => _additionalValue;
    public int ChipLevel => _chipLevel;

    #endregion

    public DamageData(DamageTypes[] damageTypes, int[] damage, int impactDamage, float[] resistance, float[] weakness, float[] secondaryValue, float[] additionalValue, int chipLevel)
    {
        _damageTypes = damageTypes;
        _damage = damage;
        _impactDamage = impactDamage;
        _weakness = weakness;
        _resistance = resistance;
        _secondaryValue = secondaryValue;
        _additionalValue = additionalValue;
        _chipLevel = chipLevel;
    }

    public DamageData(DamageData damageData)
    {
        _damageTypes = damageData.DamageTypes;
        _damage = damageData.Damage;
        _impactDamage = damageData.ImpactDamage;
        _weakness = damageData.Weakness;
        _resistance = damageData.Resistance;
        _secondaryValue = damageData.SecondaryValue;
        _additionalValue = damageData.AdditionalValue;
        _chipLevel = damageData.ChipLevel;
    }

    public static DamageData GetDamageData(DamageTypes[] damageTypes, Weapon weapon, int aiCombatID = 0)
    {
        if (aiCombatID < 0 && weapon == null) 
        {
            int[] damage = { 20, 0 };
            float[] nullFloatArray = { 0, 0 };
            return new DamageData(damageTypes, damage, 0, nullFloatArray, nullFloatArray, nullFloatArray, nullFloatArray, 1); 
        }

        DamageData damageData = null;

        if (weapon == null) 
        {
            //DamageSO for buildings
            if(aiCombatID >= 100) { GameAssetsManager.Instance.BuildingDamageDataSO.CreateDamageForEnemies(damageTypes, aiCombatID, ref damageData); }
            //DamageSO for enemies
            else { GameAssetsManager.Instance.EnemyDamageDataSO.CreateDamageForEnemies(damageTypes, aiCombatID, ref damageData); }
        }
        //DamageSO for chip upgradables
        else { GameAssetsManager.Instance.ChipDataSO.CreateDamageDataFromChip(damageTypes, weapon, ref damageData); }

        return damageData;
    }
}