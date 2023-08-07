using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTransformBehavior : MonoBehaviour
{
    #region Private Variables

    private GameObject _shipMoving;
    private List<Transform> _enemySpawnTransforms = new List<Transform>();
    private static EnemyTransformBehavior _instance;

    #endregion

    #region Public Properties

    public List<Transform> EnemySpawnTransforms => _enemySpawnTransforms;
    public static EnemyTransformBehavior Instance { get { return _instance; } }

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
        
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            _enemySpawnTransforms.Add(gameObject.transform.GetChild(i));
        }
    }

    private void Update()
    {
        CheckForShipDistance();
    }

    #endregion

    private void CheckForShipDistance()
    {
        if (!EnemyWaveSystem.Instance.IsNightTime) { return; }

        foreach (Transform enemySpawnTransform in _enemySpawnTransforms)
        {
            if ((Vector2.Distance(new Vector2(enemySpawnTransform.position.x, enemySpawnTransform.position.y), new Vector2(_shipMoving.transform.position.x, _shipMoving.transform.position.y)) < 80) && enemySpawnTransform.gameObject.activeInHierarchy)
            {
                enemySpawnTransform.gameObject.SetActive(false);
            }
            else if ((Vector2.Distance(new Vector2(enemySpawnTransform.position.x, enemySpawnTransform.position.y), new Vector2(_shipMoving.transform.position.x, _shipMoving.transform.position.y)) > 80) && !enemySpawnTransform.gameObject.activeInHierarchy)
            {
                enemySpawnTransform.gameObject.SetActive(true);
            }
        }
    }
}