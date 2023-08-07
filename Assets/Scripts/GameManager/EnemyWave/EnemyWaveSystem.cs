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

    private bool _isNightTime = true;
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
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion
}