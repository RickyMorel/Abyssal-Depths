using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFieldOfView : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private List<AICombat> _enemyAiList = new List<AICombat>();

    #endregion

    #region Private Variables

    private bool _hasEnemy = false;

    #endregion

    private void OnTriggerStay(Collider other)
    {
        if(!other.TryGetComponent<Ship>(out Ship ship)) { return; }

        foreach (AICombat ai in _enemyAiList)
        {
            if(ai.gameObject.activeInHierarchy == false) { continue; }

            ai.Aggro();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out AICombat enemyAi)) { return; }

        foreach (AICombat enemyList in _enemyAiList)
        {
            if (enemyList.Equals(enemyAi))
            {
                _hasEnemy = true;
            }
        }

        if (!_hasEnemy) 
        { 
            _enemyAiList.Add(enemyAi);
        }

        _hasEnemy = false;
    }
}