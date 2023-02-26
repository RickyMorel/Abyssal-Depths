using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Combat/EnemyDamageData")]

public class EnemyDamageDataSO : ScriptableObject
{
    public List<EnemyDamageValues> EnemyDataList = new List<EnemyDamageValues>();
    public Dictionary<int, EnemyDamageValues> EnemyDataDictionary = new Dictionary<int, EnemyDamageValues>();

    public EnemyDamageDataSO()
    {
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
    public int[] Damage = { 0, 0 };
    public int ImpactDamage;
    public DamageType[] DamageType = { 0, 0 };
    [Tooltip("This is used for maximumdamage, burntime, paralysistime, timetoreachmaxdamage")]
    public float[] SecondaryValue = { 0, 0 };
    [Tooltip("This is used for timebetweenburns, stuntime, timebetweenhits")]
    public float[] AdditionalValue = { 0, 0 };
}