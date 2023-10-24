using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// If the ship is near a spawn position, this script will deactivate said spawn position, that way enemies don't spawn in the player's face 
/// </summary>

public class EnemySpawnPositionBehaviour : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private List<DayEnemySpawnPositionsClass> EnemySpawnPositionPerNight = new List<DayEnemySpawnPositionsClass>();

    #endregion

    #region Private Variables

    private static EnemySpawnPositionBehaviour _instance;

    #endregion

    #region Public Properties

    public static EnemySpawnPositionBehaviour Instance { get { return _instance; } }

    #endregion

    #region UnityLoops

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion

    public List<Transform> GetWhereToSpawn()
    {
        return EnemySpawnPositionPerNight[DayNightManager.Instance.DayCount - 1].EnemySpawnPosition;
    }

    #region Helper Class

    [Serializable]
    public class DayEnemySpawnPositionsClass
    {
        public List<Transform> EnemySpawnPosition;
    }

    #endregion
}