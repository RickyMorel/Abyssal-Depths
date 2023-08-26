using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BasePart : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private Transform[] _stopLocations;

    #endregion

    #region Private Variables

    private NavMeshAgent _agent;
    private int _currentLocationIndex;

    #endregion

    #region Unity Loops

    private void Start()
    {
        DayNightManager.Instance.OnCycleChange += HandleCycleChange;

        _agent = GetComponent<NavMeshAgent>();
    }

    #endregion

    private void HandleCycleChange()
    {
        Debug.Log("HandleCycleChange: ");

        if (!DayNightManager.Instance.IsNightTime) { return; }

        GoToNextLocation();
    }

    private void GoToNextLocation()
    {
        Debug.Log("GoToNextLocation: ");

        _agent.SetDestination(_stopLocations[_currentLocationIndex].transform.position);

        _currentLocationIndex++;
    }
}
