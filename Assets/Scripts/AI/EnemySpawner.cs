using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    #region Editor Fields

    [SerializeField] private List<AICombat> _enemyWaveList = new List<AICombat>();
    [SerializeField] private AIFieldOfView _enemyFovZone;

    #endregion

    #region Private Variables

    private bool _canSpawnEnemies = false;
    private bool _shouldSpawnEnemies = false;
    private EnemySpawner _otherSpawner;
    private NextLevelTriggerZone _nextLevelTriggerZone;
    private Transform _enemySpawnDistance;

    #endregion

    #region Public Properties

    public bool CanSpawnEnemies => _canSpawnEnemies;

    #endregion

    #region Unity Loops

    private void Start()
    {
        _nextLevelTriggerZone = GetComponentInParent<NextLevelTriggerZone>();

        for (int i = 0; i < _nextLevelTriggerZone.GetComponentsInChildren<EnemySpawner>().Length; i++)
        {
            if (_nextLevelTriggerZone.GetComponentsInChildren<EnemySpawner>()[i] != this) { _otherSpawner = _nextLevelTriggerZone.GetComponentsInChildren<EnemySpawner>()[i]; }
        }
    }

    private void Update()
    {
        if (!_canSpawnEnemies) { return; }

        EnemySpawnerFunction(_enemySpawnDistance);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Ship ship)) { return; }

        if (_otherSpawner.CanSpawnEnemies || !_nextLevelTriggerZone.IsInPhase3) { return; }

        _enemySpawnDistance = ship.transform;
        _enemySpawnDistance.position = _enemySpawnDistance.position - new Vector3(-70, 0, 0);

        _canSpawnEnemies = true;

        if (_enemyFovZone)
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Ship _)) { return; }

        _canSpawnEnemies = false;
    }

    #endregion

    private void EnemySpawnerFunction(Transform transform)
    {
        _canSpawnEnemies = false;

        foreach (AICombat enemy in _enemyWaveList)
        {
            Instantiate(enemy, transform);
        }
    }
}