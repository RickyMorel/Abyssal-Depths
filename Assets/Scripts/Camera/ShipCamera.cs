using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;
using System;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class ShipCamera : BaseCamera
{
    #region Private Variables

    private static ShipCamera _instance;
    private Rigidbody _shipRigidbody;
    private Booster _shipBooster;

    private bool _isBoosting;
    private float _currentFOV;
    private float _orginalFOV;
    #endregion

    #region Public Properties

    public static ShipCamera Instance { get { return _instance; } }

    #endregion

    #region Unity Loops

    private void Awake()
    {
        Booster.OnBoostUpdated += HandleBoost;

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public override void Start()
    {
        base.Start();

        _shipRigidbody = FindObjectOfType<ShipHealth>().GetComponent<Rigidbody>();
        _shipBooster = _shipRigidbody.GetComponentInChildren<Booster>();

        _orginalFOV = _virtualCamera.m_Lens.OrthographicSize;
        _currentFOV = _orginalFOV;
    }

    private void Update()
    {
        UpdateBoostFOVEffect();
    }

    private void OnDestroy()
    {
        Booster.OnBoostUpdated -= HandleBoost;
    }

    #endregion

    private void UpdateBoostFOVEffect()
    {
        float expandedBoostFOV = _orginalFOV + (_shipRigidbody.velocity.magnitude * _expandedFovToVelocityRatio);
        float wantedFOV = _isBoosting == true && _shipBooster.RecentlyChangedGear == false ? expandedBoostFOV : _orginalFOV;

        _currentFOV = Mathf.Lerp(_currentFOV, wantedFOV, Time.deltaTime);
        _virtualCamera.m_Lens.OrthographicSize = _currentFOV;
    }

    private void HandleBoost(bool isBoosting)
    {
        _isBoosting = isBoosting;
    }

    private void ShakeWhenBoosting()
    {
        float velocityToShakeRatio = 20f;
        float shakeAmount = _isBoosting == true && _shipBooster.RecentlyChangedGear == false ? _shakeAmplitude : 0f;
        float finalShakeAmount = shakeAmount * (_shipRigidbody.velocity.magnitude / velocityToShakeRatio);
        _virtualCameraNoise.m_AmplitudeGain = finalShakeAmount;
    }
}