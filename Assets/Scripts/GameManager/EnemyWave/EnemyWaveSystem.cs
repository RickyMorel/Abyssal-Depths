using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveSystem : MonoBehaviour
{
    #region

    [SerializeField] private List<AICombat> _enemyWaveList = new List<AICombat>();

    #endregion

    #region Private Variables

    private bool _isNightTime = true;
    private bool _shouldSpawnEnemies = false;
    private bool _canEnemiesSpawn = false;
    private static EnemyWaveSystem _instance;
    private List<GameObject> _spawnedEnemies = new List<GameObject>();
    private List<Transform> _enemySpawnTranforms = new List<Transform>();

    #endregion

    #region Public Properties

    public bool IsNightTime => _isNightTime;
    public static EnemyWaveSystem Instance { get { return _instance; } }

    #endregion

    #region Unity Loops

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

    private void Start()
    {
        EnemyTransformBehavior.Instance.OnTransformCheck += GetEnemySpawnTransforms;
    }

    #endregion

    private void GetEnemySpawnTransforms()
    {
        _enemySpawnTranforms = EnemyTransformBehavior.Instance.EnemySpawnTransforms;
    }

    private void EnemyWaveSpawnsEnemies()
    {
        if (!_isNightTime) { return; }

        if (!_shouldSpawnEnemies) { return; }

        
    }

    private void EnemySpawnerFunction(List<Transform> transformsList)
    {
        if (!_shouldSpawnEnemies) { return; }

        _shouldSpawnEnemies = false;

        foreach (AICombat enemy in _enemyWaveList)
        {
            GameObject newEnemy = Instantiate(enemy, ).gameObject;

            _spawnedEnemies.Add(newEnemy);
        }
    }

    private void CheckIfAllEnemiesAreDefeated()
    {
        int count = 0;
        foreach (GameObject enemy in _spawnedEnemies)
        {
            if (enemy == null || !enemy.activeSelf)
            {
                count++;
            }
        }

        if (count == _spawnedEnemies.Count)
        {
            _shouldSpawnEnemies = true;
            _spawnedEnemies.Clear();
            EnemySpawnerFunction(_enemySpawnZone);
        }
    }
}