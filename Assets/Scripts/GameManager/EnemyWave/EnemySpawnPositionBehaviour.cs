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

    [SerializeField] private List<Transform> _enemySpawnTransforms = new List<Transform>();

    #endregion

    #region Private Variables

    private GameObject _shipMoving;
    private static EnemySpawnPositionBehaviour _instance;

    #endregion

    #region Public Properties

    public List<Transform> EnemySpawnTransforms => _enemySpawnTransforms;
    public static EnemySpawnPositionBehaviour Instance { get { return _instance; } }
    public event Action OnTransformCheck;

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

    private void Start()
    {
        _shipMoving = GameObject.FindGameObjectWithTag("MainShip");
        OnTransformCheck?.Invoke();

        EnemyWaveSystem.Instance.OnEnemyAboutToSpawn += CheckForShipDistance;
    }

    private void OnDisable()
    {
        EnemyWaveSystem.Instance.OnEnemyAboutToSpawn -= CheckForShipDistance;
    }

    #endregion

    private void CheckForShipDistance()
    {
        if (!DayNightManager.Instance.IsNightTime) { return; }

        foreach (Transform enemySpawnTransform in _enemySpawnTransforms)
        {
            if ((Vector2.Distance(new Vector2(enemySpawnTransform.position.x, enemySpawnTransform.position.y), new Vector2(_shipMoving.transform.position.x, _shipMoving.transform.position.y)) < 80) && enemySpawnTransform.gameObject.activeInHierarchy)
            {
                enemySpawnTransform.gameObject.SetActive(false);
                OnTransformCheck?.Invoke();
            }
            else if ((Vector2.Distance(new Vector2(enemySpawnTransform.position.x, enemySpawnTransform.position.y), new Vector2(_shipMoving.transform.position.x, _shipMoving.transform.position.y)) > 80) && !enemySpawnTransform.gameObject.activeInHierarchy)
            {
                enemySpawnTransform.gameObject.SetActive(true);
                OnTransformCheck?.Invoke();
            }
        }
    }
}