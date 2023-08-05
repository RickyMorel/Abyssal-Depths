using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTransformBehavior : MonoBehaviour
{
    #region Public Properties

    public event Action OnCheckForTransforms;

    #endregion

    #region UnityLoops

    private void Update()
    {
        CheckForShipDistance();
    }

    private void OnEnable()
    {
        OnCheckForTransforms?.Invoke();
    }

    private void OnDisable()
    {
        OnCheckForTransforms?.Invoke();
    }

    #endregion

    private void CheckForShipDistance()
    {
        if (!EnemyWaveSystem.Instance.IsNightTime) { return; }


    }
}