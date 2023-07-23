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
}