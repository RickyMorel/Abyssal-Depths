using FMOD.Studio;
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
    private EventInstance _boostSfx;

    #endregion

    #region Unity Loops

    private void Start()
    {
        BasePartsManager.Instance.OnLocationChanged += GoToNextLocation;

        _boostSfx = GameAudioManager.Instance.CreateSoundInstance(GameAudioManager.Instance.BoosterBoostSfx, Ship.Instance.transform);

        _boostSfx.start();
    }

    private void Update()
    {
        UpdateBoostSfx();

        //if(_currentLocationTransform != null) { _agent.SetDestination(_currentLocationTransform.position); }

        if (_agent.pathPending) { return; }

        if (Vector3.Distance(transform.position, _currentLocationTransform.position) > _agent.stoppingDistance) { return; }

        if (_agent.hasPath || _agent.velocity.sqrMagnitude != 0f) { return; }

        _baseObj.transform.position = _currentLocationTransform.transform.position;
        _baseObj.transform.rotation = _currentLocationTransform.transform.rotation;
        _baseObj.gameObject.SetActive(true);

        Destroy(gameObject); 
    }

    private void OnDestroy()
    {
        BasePartsManager.Instance.OnLocationChanged -= GoToNextLocation;

        _boostSfx.stop(STOP_MODE.ALLOWFADEOUT);
    }

    #endregion

    private void UpdateBoostSfx()
    {
        float speedPercentage = Mathf.Clamp((_agent.velocity.magnitude * 1.5f) / _agent.desiredVelocity.magnitude, 0f, 1f);

        GameAudioManager.Instance.AdjustAudioParameter(_boostSfx, "BoostSpeed", speedPercentage);
    }

    public void Initialize(GameObject baseObj, BasePartType basePartType)
    {
        _agent = GetComponent<NavMeshAgent>();
        _baseObj = baseObj;
        _partType = basePartType;

        transform.position = baseObj.transform.position;
        transform.position = Utils.RandomNavmeshLocation(_agent, 50f, 20f, 50f, NavMesh.AllAreas);

        GoToNextLocation();
    }

    private void GoToNextLocation()
    {
        gameObject.SetActive(true);
        _baseObj.SetActive(false);

        _currentLocationTransform = BasePartsManager.Instance.GetLocation(_partType);

        _agent.SetDestination(_currentLocationTransform.position);
    }
}
