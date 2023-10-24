using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveSystem : MonoBehaviour
{
    #region

    [SerializeField] private List<AICombat> _enemyPrefabs = new List<AICombat>();
    [SerializeField] private int _howManyEnemiesShouldSpawnAtOnce = 5;
    [SerializeField] private float _timeBetweenEnemySpawns = 5;

    #endregion

    #region Private Variables

    private static EnemyWaveSystem _instance;
    private List<Transform> _enemySpawnTranforms = new List<Transform>();
    private float _timer = 0;
    private List<AIHealth> _currentNightEnemies = new List<AIHealth>();

    #endregion

    #region Public Properties

    public static EnemyWaveSystem Instance { get { return _instance; } }
    public event Action OnEnemyAboutToSpawn;

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

        DayNightManager.Instance.OnCycleChange += KillCurrentNightEnemiesIfDayTime;
        DayNightManager.Instance.OnCycleChange += CheckIfEnemiesShouldSpawn;
        DayNightManager.Instance.OnCycleChange += GetEnemySpawnTransforms;
    }

    private void OnDisable()
    {
        DayNightManager.Instance.OnCycleChange -= KillCurrentNightEnemiesIfDayTime;
        DayNightManager.Instance.OnCycleChange -= CheckIfEnemiesShouldSpawn;
        DayNightManager.Instance.OnCycleChange -= GetEnemySpawnTransforms;
    }

    private void Update()
    {
        if (DayNightManager.Instance.IsNightTime) { CheckIfEnemiesShouldSpawn(); }
    }

    #endregion

    private void GetEnemySpawnTransforms()
    {
        _enemySpawnTranforms = EnemySpawnPositionBehaviour.Instance.GetWhereToSpawn();
    }
    private void CheckIfEnemiesShouldSpawn()
    {
        _timer += Time.deltaTime;

        if (_timer < _timeBetweenEnemySpawns) { return; }

        _timer = 0;
        EnemySpawnerFunction();
    }

    private void EnemySpawnerFunction()
    {
        OnEnemyAboutToSpawn?.Invoke();

        for (int i = 0; i < _howManyEnemiesShouldSpawnAtOnce; i++)
        {
            
            GameObject newEnemy = Instantiate(_enemyPrefabs[UnityEngine.Random.Range(0, _enemyPrefabs.Count)], _enemySpawnTranforms[UnityEngine.Random.Range(0, _enemySpawnTranforms.Count)]).gameObject;
            newEnemy.transform.SetParent(null);
            _currentNightEnemies.Add(newEnemy.GetComponent<AIHealth>());
        }
    }

    private void KillCurrentNightEnemiesIfDayTime()
    {
        if (DayNightManager.Instance.IsNightTime) { return; }

        foreach (AIHealth enemy in _currentNightEnemies)
        {
            enemy.DamageWithoutDamageData(99999999);
        }
    }
}