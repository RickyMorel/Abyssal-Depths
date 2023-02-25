using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Combat/EnemyDamageData")]

public class EnemyDamageDataSO : ScriptableObject
{
    EnemyDamageValues _enemy = new EnemyDamageValues();
    List<EnemyDamageValues> _enemyDataList = new List<EnemyDamageValues>();
    Dictionary<int, EnemyDamageValues> _enemyDataDictionary = new Dictionary<int, EnemyDamageValues>();

    public EnemyDamageDataSO()
    {

    }
}

public class EnemyDamageValues
{
    public int EnemyID;
    public int Damage;
    public DamageType DamageType;
}