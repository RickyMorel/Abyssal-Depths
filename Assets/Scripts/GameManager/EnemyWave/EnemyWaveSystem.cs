using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveSystem : MonoBehaviour
{
    #region

    [SerializeField] private List<AICombat> _enemyPrefabs = new List<AICombat>();

    #endregion

    #region Private Variables

    private float _timer = 0;
    private int _howManyEnemiesShouldSpawn = 5;
    private bool _isNightTime = true;
    private bool _shouldSpawnEnemies = true;
    private static EnemyWaveSystem _instance;
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

        EnemyTransformBehavior.Instance.OnTransformCheck += GetEnemySpawnTransforms;
    }

    private void Update()
    {
        CheckIfEnemiesShouldSpawn();
    }

    #endregion

    private void GetEnemySpawnTransforms()
    {
        _enemySpawnTranforms = EnemyTransformBehavior.Instance.EnemySpawnTransforms;
    }

    private void EnemySpawnerFunction()
    {
        if (!_shouldSpawnEnemies) { return; }

        _shouldSpawnEnemies = false;

        for (int i = 0; i < _howManyEnemiesShouldSpawn; i++)
        {
            
            GameObject newEnemy = Instantiate(_enemyPrefabs[Random.Range(0, _enemyPrefabs.Count)], _enemySpawnTranforms[Random.Range(0, _enemySpawnTranforms.Count)]).gameObject;
            newEnemy.transform.SetParent(null);
        }
    }

    private void CheckIfEnemiesShouldSpawn()
    {
        if (!_isNightTime) { return; }

        _timer += Time.deltaTime;

        if (_timer < 5) { return; }

        _timer = 0;
        
        EnemySpawnerFunction();
    }
}