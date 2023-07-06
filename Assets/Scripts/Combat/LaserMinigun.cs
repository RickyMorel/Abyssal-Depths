using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserMinigun : WeaponShoot
{
    #region Private Variables

    private int _shootNumber = -1;
    private Transform _barrel;
    private float _rotationSpeed = 0;
    private bool _isShooting;

    #endregion

    #region Editor Fields

    [SerializeField] private float _maxRotationSpeed = 500;

    #endregion

    #region Unity Loops

    public override void Start()
    {
        base.Start();
        _barrel = _weapon.TurretHead.GetChild(0);
    }

    public override void Update()
    {
        base.Update();

        if (_isShooting) { WeaponBarrelRoll(_maxRotationSpeed); }
    }

    #endregion

    public override void CheckShootInput()
    {
        if (_weapon.CurrentPlayer.IsUsing)
        {
            Shoot();
        }
        else
        {
            _isShooting = false;
            if (_rotationSpeed <= 0) { return; }
            _rotationSpeed = _rotationSpeed - Time.deltaTime * 200;
            WeaponBarrelRoll(_rotationSpeed);
        }
    }

    public override void Shoot()
    {
        if (_timeBetweenShots > _timeSinceLastShot) { return; }

        if (_rotationSpeed < _maxRotationSpeed)
        {
            _rotationSpeed = _rotationSpeed + Time.deltaTime * 200;
            WeaponBarrelRoll(_rotationSpeed);
            return;
        }

        _isShooting = true;

        PlayShootFX();

        if (_shootNumber < _weapon.ShootTransforms.Count - 1) 
        { 
            _shootNumber++; 
            ProjectileShootFromOtherBarrels(_shootNumber);
            _timeSinceLastShot = 0f;
        }

        else { _shootNumber = -1; }
    }

    private void WeaponBarrelRoll(float rotationSpeed)
    {
        _barrel.Rotate(new Vector3(0f, 0f, rotationSpeed * Time.deltaTime));
    }
}