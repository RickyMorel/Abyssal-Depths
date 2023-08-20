using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveSystem : MonoBehaviour
{
    #region

    [SerializeField] private List<AICombat> _enemyPrefabs = new List<AICombat>();
    [SerializeField] private int _howManyEnemiesShouldSpawnAtOnce = 5;

    #endregion

    #region Private Variables

    private static EnemyWaveSystem _instance;
    private List<Transform> _enemySpawnTranforms = new List<Transform>();
    private float _timer = 0;
    private bool _shouldSpawnEnemies = true;

    #endregion

    #region Public Properties

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

        EnemySpawnPositionBehaviour.Instance.OnTransformCheck += GetEnemySpawnTransforms;
    }

    private void Update()
    {
        CheckIfEnemiesShouldSpawn();
    }

    #endregion

    private void GetEnemySpawnTransforms()
    {
        _enemySpawnTranforms = EnemySpawnPositionBehaviour.Instance.EnemySpawnTransforms;
    }

    private void EnemySpawnerFunction()
    {
        if (!_shouldSpawnEnemies) { return; }

        _shouldSpawnEnemies = false;

        for (int i = 0; i < _howManyEnemiesShouldSpawnAtOnce; i++)
        {
            GameObject newEnemy = Instantiate(_enemyPrefabs[Random.Range(0, _enemyPrefabs.Count)], _enemySpawnTranforms[Random.Range(0, _enemySpawnTranforms.Count)]).gameObject;
            newEnemy.transform.SetParent(null);
        }
    }
    
    private void CheckIfEnemiesShouldSpawn()
    {
        if (DayNightManager.Instance.IsNightTime) { return; }

        _timer += Time.deltaTime;

        if (_timer < 5) { return; }

        _timer = 0;
        
        EnemySpawnerFunction();
    }
}