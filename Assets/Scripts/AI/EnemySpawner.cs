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

    private bool _canSpawnEnemies = false;
    private bool _shouldSpawnEnemies = false;
    private NextLevelTriggerZone _nextLevelTriggerZone;

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
            Instantiate(enemy, transform);
        }
    }

    private void CheckIfAllEnemiesAreDefeated()
    {
        int count = 0;
        foreach (AICombat enemy in _enemyFovZone.EnemyAiList)
        {
            if (enemy == null || !enemy.gameObject.activeSelf)
            {
                count++;
            }
        }
        if (count == _enemyFovZone.EnemyAiList.Count)
        {
            _shouldSpawnEnemies = true; 
            _enemyFovZone.ClearEnemyList(); 
            EnemySpawnerFunction(_enemySpawnZone); 
        }
    }
}