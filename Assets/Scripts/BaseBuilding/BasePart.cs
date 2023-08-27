using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Codice.Client.Common.WebApi.WebApiEndpoints;

[RequireComponent(typeof(NavMeshAgent))]
public class BasePart : MonoBehaviour
{
    #region Private Variables

    private NavMeshAgent _agent;
    private Transform _currentLocationTransform;
    private GameObject _baseObj;
    private BasePartType _partType;

    #endregion

    #region Unity Loops

    private void Update()
    {
        if (_agent.pathPending) { return; }

        Debug.Log($"{Vector3.Distance(transform.position, _currentLocationTransform.position)} > {_agent.stoppingDistance}");

        if (Vector3.Distance(transform.position, _currentLocationTransform.position) > _agent.stoppingDistance) { return; }

        if (_agent.hasPath || _agent.velocity.sqrMagnitude != 0f) { return; }

        Debug.Log("Reached Destination!");

        _baseObj.transform.position = _currentLocationTransform.transform.position;
        _baseObj.transform.rotation = _currentLocationTransform.transform.rotation;
        _baseObj.gameObject.SetActive(true);

        Destroy(gameObject); 
    }

    #endregion

    public void Initialize(GameObject baseObj, BasePartType basePartType)
    {
        _agent = GetComponent<NavMeshAgent>();

        gameObject.SetActive(true);
        baseObj.SetActive(false);
        _baseObj = baseObj;
        _partType = basePartType;

        transform.position = baseObj.transform.position;
        transform.position = Utils.RandomNavmeshLocation(_agent, 20f, 10f, 20f, NavMesh.AllAreas);

        GoToNextLocation();
    }

    private void GoToNextLocation()
    {
        _currentLocationTransform = BasePartsManager.Instance.GetLocation(_partType);

        Debug.Log("_currentLocationTransform: " + _currentLocationTransform.gameObject.name);

        _agent.SetDestination(_currentLocationTransform.position);
    }
}
