using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Combat/EnemyDamageData")]

public class EnemyDamageDataSO : ScriptableObject
{
    private DamageType _damageMultipliers;
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