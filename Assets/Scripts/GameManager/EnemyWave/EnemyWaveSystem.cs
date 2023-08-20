using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveSystem : MonoBehaviour
{
    #region

    [SerializeField] private List<AICombat> _enemyPrefabs = new List<AICombat>();
    [Tooltip("This is measured in seconds")]
    [SerializeField] private int _howLongTheDayLast = 6;
    [SerializeField] private int _howLongTheNightLast = 6;
    [SerializeField] private float _lowFogDensity = 0.01f;
    [SerializeField] private float _highFogDensity = 0.03f;

    #endregion

    #region Private Variables

    private static EnemyWaveSystem _instance;
    private List<Transform> _enemySpawnTranforms = new List<Transform>();
    [Header("Integers")]
    private int _howManyEnemiesShouldSpawnAtOnce = 5;
    private int _dayCount = 2;
    [Header("Floats")]
    private float _timer = 0;
    private float _dayTimer = 0;
    private float _nightTimer = 0;
    [Header("Bools")]
    private bool _isNightTime = false;
    private bool _shouldSpawnEnemies = true;

    #endregion

    #region Public Properties

    public bool IsNightTime => _isNightTime;
    public int DayCount => _dayCount;
    public static EnemyWaveSystem Instance { get { return _instance; } }
    public event Action OnCycleChange;

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
        DayNightCycle();
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

        for (int i = 0; i < _howManyEnemiesShouldSpawnAtOnce; i++)
        {
            GameObject newEnemy = Instantiate(_enemyPrefabs[UnityEngine.Random.Range(0, _enemyPrefabs.Count)], _enemySpawnTranforms[UnityEngine.Random.Range(0, _enemySpawnTranforms.Count)]).gameObject;
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

    private void DayNightCycle()
    {
        if (!_isNightTime)
        {
            _dayTimer += Time.deltaTime;

            RenderSettings.fogDensity = Mathf.Lerp(_highFogDensity, _lowFogDensity, _dayTimer);

            if (_dayTimer >= _howLongTheDayLast) 
            {
                _isNightTime = true;
                OnCycleChange?.Invoke();
                _timer = 0;
                _dayTimer = 0;
            }
        }
        if (_isNightTime)
        {
            _nightTimer += Time.deltaTime;

            RenderSettings.fogDensity = Mathf.Lerp(_lowFogDensity, _highFogDensity, _nightTimer);

            if (_nightTimer >= _howLongTheNightLast) 
            {
                _isNightTime = false;
                OnCycleChange?.Invoke();
                _nightTimer = 0;
                _dayCount += 1;
            }
        }
    }
}