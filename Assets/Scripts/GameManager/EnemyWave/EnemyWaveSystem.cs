using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveSystem : MonoBehaviour
{
    #region

    [SerializeField] private List<GameObject> _enemyWave = new List<GameObject>();
    [SerializeField] private List<GameObject> _enemyPrefabs = new List<GameObject>();

    #endregion

    #region Private Variables

    private List<EnemyTransformBehavior> _enemySpawnTransforms = new List<EnemyTransformBehavior>();
    private bool _isNightTime = false;
    private static EnemyWaveSystem _instance;

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
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        GetAllTransforms();
    }

    private void OnDestroy()
    {
        for (int i = 0; i < _enemySpawnTransforms.Count; i++)
        {
            _enemySpawnTransforms[i].OnCheckForTransforms -= GetAllTransforms;
        }
    }

    #endregion

    private void GetAllTransforms()
    {
        _enemySpawnTransforms.Clear();

        EnemyTransformBehavior[] enemyTransforms = GetComponentsInChildren<EnemyTransformBehavior>();

        for (int i = 0; i < enemyTransforms.Length; i++)
        {
            _enemySpawnTransforms.Add(enemyTransforms[i]);
            _enemySpawnTransforms[i].OnCheckForTransforms += GetAllTransforms;
        }
    }
}