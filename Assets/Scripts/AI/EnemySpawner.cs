using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    #region Editor Fields

    [SerializeField] private List<AICombat> _enemyWaveList = new List<AICombat>();
    [SerializeField] private AIFieldOfView _enemyFovZone;
    [SerializeField] private Transform _enemySpawnZone;

    #endregion

    #region Private Variables

    [SerializeField] private bool _canSpawnEnemies = false;
    [SerializeField] private bool _shouldSpawnEnemies = false;
    private NextLevelTriggerZone _nextLevelTriggerZone;
    private List<GameObject> _spawnedEnemies = new List<GameObject>();

    #endregion

    #region Unity Loops

    private void Start()
    {
        _nextLevelTriggerZone = GetComponentInParent<NextLevelTriggerZone>();
    }

    private void Update()
    { 
        if (!_canSpawnEnemies) { return; }

        CheckIfAllEnemiesAreDefeated();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Ship _)) { return; }

        Debug.Log("EnemySpawner: " + other.gameObject.name);

        if (!_nextLevelTriggerZone.IsInPhase3) { return; }
        
        _canSpawnEnemies = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Ship _)) { return; }

        _canSpawnEnemies = false;
    }

    #endregion

    private void EnemySpawnerFunction(Transform transform)
    {
        if (!_shouldSpawnEnemies) { return; }

        _shouldSpawnEnemies = false;

        foreach (AICombat enemy in _enemyWaveList)
        {
            GameObject newEnemy = Instantiate(enemy, transform).gameObject;

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