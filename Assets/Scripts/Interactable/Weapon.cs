using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Upgradable
{
    #region Editor Fields

    [SerializeField] private int _weaponId = -1;

    [Header("Rotation Variables")]
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private Vector2 _rotationLimits;

    #endregion

    #region Private Variables

    private WeaponHumble _weaponHumble;
    private float _rotationX;
    private bool _shouldRotate = true;

    #endregion

    #region Public Properties

    public int WeaponId => _weaponId;
    public GameObject ProjectilePrefab => _weaponHumble.ProjectilePrefab;
    public List<Transform> ShootTransforms => _weaponHumble.ShootTransforms;
    public Transform TurretHead => _weaponHumble.TurretHead;

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

        _rotationX = (_rotationLimits.x + _rotationLimits.y)/2;
    }

    private void Update()
    {
        if (CurrentPlayer == null) { return; }
        
        if (CanUse == false) { return; }

        _weaponHumble?.WeaponShoot?.CheckShootInput();
        CheckRotationInput();
    }

    private void OnDestroy()
    {
        OnUpgradeMesh -= _weaponHumble.HandleUpgrade;
    }

    #endregion

    private void CheckRotationInput()
    {
        if(!DoesRotate() || !_shouldRotate) { return; }

        if (CurrentPlayer.MoveDirection.x == 0) { return; }

        _weaponHumble.TurretHead.localEulerAngles = _weaponHumble.CalculateWeaponLocalRotation(ref _rotationX, CurrentPlayer.MoveDirection.x, _rotationSpeed, _rotationLimits);
    }

    private bool DoesRotate()
    {
        if(_selectedUpgrade == null) { return true; }

        //if is harpoon gun or lightsaber
        if (_selectedUpgrade.UpgradeSO.Socket_1 == ChipType.Base && _selectedUpgrade.UpgradeSO.Socket_2 == ChipType.Electric) { return true; }
        if (_selectedUpgrade.UpgradeSO.Socket_1 == ChipType.Electric && _selectedUpgrade.UpgradeSO.Socket_2 == ChipType.Base) { return true; }
        if (_selectedUpgrade.UpgradeSO.Socket_1 == ChipType.Base && _selectedUpgrade.UpgradeSO.Socket_2 == ChipType.Laser) { return true; }
        if (_selectedUpgrade.UpgradeSO.Socket_1 == ChipType.Laser && _selectedUpgrade.UpgradeSO.Socket_2 == ChipType.Base) { return true; }

        //if is any other base weapon
        if (_selectedUpgrade.UpgradeSO.Socket_1 == ChipType.Base || _selectedUpgrade.UpgradeSO.Socket_2 == ChipType.Base) { return false; }

        return true;
    }
}