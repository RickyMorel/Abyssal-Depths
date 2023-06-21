using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFieldOfView : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private List<AICombat> _aiList = new List<AICombat>();

    #endregion

    #region Private Variables

    private bool _hasAiInList = false;

    #endregion

    #region Public Properties

    public List<AICombat> AiList => _aiList;

    #endregion

    private void OnTriggerStay(Collider other)
    {
        if(!other.TryGetComponent<Ship>(out Ship ship)) { return; }

        foreach (AICombat ai in _aiList)
        {
            if(ai.gameObject.activeInHierarchy == false) { continue; }

            ai.Aggro();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out AICombat enemyAi)) { return; }

        foreach (AICombat enemyList in _aiList)
        {
            if (enemyList.Equals(enemyAi))
            {
                _hasAiInList = true;
            }
        }

        if (!_hasAiInList) 
        { 
            _aiList.Add(enemyAi);
        }

        _hasAiInList = false;
    }

    public void ClearEnemyList()
    {
        _aiList.Clear();
    }
}