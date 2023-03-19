using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Combat/EnemyDamageData")]

public class EnemyDamageDataSO : ScriptableObject
{
    public List<EnemyDamageValues> EnemyDataList = new List<EnemyDamageValues>();
    public Dictionary<int, EnemyDamageValues> EnemyDataDictionary = new Dictionary<int, EnemyDamageValues>();

    public EnemyDamageDataSO(EnemyDamageDataSO enemyDamageDataSO)
    {
        EnemyDataList = enemyDamageDataSO.EnemyDataList;
        foreach (EnemyDamageValues enemyDamageData in EnemyDataList)
        {
            EnemyDataDictionary.Add(enemyDamageData.EnemyID, enemyDamageData);
        }
    }

    public void CreateDamageForEnemies(DamageTypes[] damageTypes, int aiCombatID, ref DamageData damageData)
    {
        int impactDamage;
        float[] weakness = { 0, 0 };
        float[] resistance = { 0, 0 };
        float[] secondaryValue = { 0, 0 };
        float[] additionalValue = { 0, 0 };
        int[] damage = { 0, 0 };
        int chipLevel = 1;
        EnemyDataDictionary.TryGetValue(aiCombatID, out EnemyDamageValues enemyDamageValues);
        for (int i = 0; i < 2; i++)
        {
            if (enemyDamageValues == null) { Debug.LogError("Check the enemy ID"); }
            
            damage[i] = enemyDamageValues.Damage[i];
            secondaryValue[i] = enemyDamageValues.SecondaryValue[i];
            additionalValue[i] = enemyDamageValues.AdditionalValue[i];
            damageTypes[i] = enemyDamageValues.DamageType[i];
            GameAssetsManager.Instance.DamageType.GetWeaknessAndResistance(damageTypes[i], out weakness[i], out resistance[i]);
        }
        impactDamage = enemyDamageValues.ImpactDamage;
        damageData = new DamageData(damageTypes, damage, impactDamage, resistance, weakness, secondaryValue, additionalValue, chipLevel);
    }
}

[System.Serializable]
public class EnemyDamageValues
{
    public int EnemyID;
    public int[] Damage;
    public int ImpactDamage;
    public DamageTypes[] DamageType;
    [Tooltip("This is used for maximumdamage, burntime, paralysistime, timetoreachmaxdamage")]
    public float[] SecondaryValue;
    [Tooltip("This is used for timebetweenburns, stuntime, timebetweenhits")]
    public float[] AdditionalValue;
}