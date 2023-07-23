using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : RotationalInteractable
{
    #region Editor Fields

    [SerializeField] private int _weaponId = -1;
    [SerializeField] private WeaponHeadID _weaponHead;

    [Header("Rotation Variables")]
    [SerializeField] private Vector2 _rotationLimits;
    [SerializeField] private Transform _rotationChecker;

    #endregion

    #region Private Variables

    private WeaponHumble _weaponHumble;
    private bool _shouldRotate = true;

    #endregion

    #region Public Properties

    public int WeaponId => _weaponId;
    public GameObject ProjectilePrefab => _weaponHumble.ProjectilePrefab;
    public Transform TurretHead => _weaponHumble.TurretHead;
    public WeaponHeadID WeaponHeadIdObj => _weaponHead;

    #endregion

    #region Getters and Setters

    public bool ShouldRotate { get { return _shouldRotate; } set { _shouldRotate = value; } }

    #endregion

    #region Unity Loops

    public override void Awake()
    {
        base.Awake();

        _humble = new WeaponHumble(IsAIOnlyInteractable);
        _weaponHumble = _humble as WeaponHumble;

        OnUpgradeMesh += _weaponHumble.HandleUpgrade;
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();

        _weaponHumble?.WeaponShoot?.CheckShootInput();

        if (Input.GetKeyDown(KeyCode.O))
        {
            Transform newWeapon = _weaponHead.SwapWeaponId().transform;
            SetRotatorTransform(newWeapon);
        }
    }

    private void OnDestroy()
    {
        OnUpgradeMesh -= _weaponHumble.HandleUpgrade;
    }

    #endregion

    public override void Rotate()
    {
        if (CurrentPlayer.MoveDirection.magnitude == 0) { return; }
        
        _currentAngle = _rotationSpeed * CurrentPlayer.MoveDirection.x * Time.deltaTime;
        _rotationChecker.RotateAround(_pivotTransform.position, Vector3.forward, -_currentAngle);

        if (_rotationChecker.localEulerAngles.x <= 10 || _rotationChecker.localEulerAngles.x >= 170) 
        {
            _rotationChecker.position = RotatorTransform.position;
            _rotationChecker.rotation = RotatorTransform.rotation;
            return; 
        }
        
        RotatorTransform.RotateAround(_pivotTransform.position, Vector3.forward, -_currentAngle);
        _rotationChecker.position = RotatorTransform.position;
        _rotationChecker.rotation = RotatorTransform.rotation;
    }
}