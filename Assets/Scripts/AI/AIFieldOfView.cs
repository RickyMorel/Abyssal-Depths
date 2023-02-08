using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFieldOfView : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private AICombat[] _enemyAiList;

    #endregion

    private void OnTriggerStay(Collider other)
    {
        if(!other.TryGetComponent<Ship>(out Ship ship)) { return; }

        foreach (AICombat ai in _enemyAiList)
        {
            ai.Aggro();
        }
    }
}
