using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;
using System;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class ShipCamera : BaseCamera
{
    #region Editor Fields

    [SerializeField] private Camera _perspectiveCamera;

    #endregion

    #region Private Variables

    private static ShipCamera _instance;
    private Rigidbody _shipRigidbody;
    private Booster _shipBooster;

    private bool _isBoosting;
    private float _currentFOV;
    private float _orginalFOV;
    private float _targetPerspectiveZ;
    private float _targetOrthoZ;
    #endregion

    #region Public Properties

    public static ShipCamera Instance { get { return _instance; } }
    public Camera PerspectiveCamera => _perspectiveCamera;

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
        _shipBooster = _shipRigidbody.transform.root.GetComponentInChildren<Booster>();

        _orginalFOV = _virtualCamera.m_Lens.OrthographicSize;
        _currentFOV = _orginalFOV;
        
        ChangeToBossZoom(true);
    }

    private void LateUpdate()
    {
        UpdateBoostFOVEffect();
        UpdateCameraZoom();
    }

    private void OnDestroy()
    {
        Booster.OnBoostUpdated -= HandleBoost;
    }

    #endregion

    private void UpdateCameraZoom()
    {
        float zoomSpeed = 5f;
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, new Vector3(0f, 0f, _targetPerspectiveZ), Time.deltaTime * zoomSpeed);
        _perspectiveCamera.transform.localPosition = Vector3.MoveTowards(_perspectiveCamera.transform.localPosition, new Vector3(0f, 0f, _targetPerspectiveZ), Time.deltaTime * zoomSpeed);
    }

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

    public void ChangeToBossZoom(bool isBossZoom)
    {
        if (isBossZoom)
        {
            _targetOrthoZ = -10.85f;
            _targetPerspectiveZ = -40.65f;
            _orginalFOV = 30f;
        }
        else
        {
            _targetOrthoZ = -4f;
            _targetPerspectiveZ = -18.95f;
            _orginalFOV = 15f;
        }
    }
}