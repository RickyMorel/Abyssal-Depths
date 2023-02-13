using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireShotgun : WeaponShoot
{
    #region Private Variable

    private bool _hasAlreadyShot = false;
    private ChipDataSO _chipDataSO;
    private ChipDataSO.BasicChip _chipClass;

    #endregion

    #region Editor Fields

    [SerializeField] private float _shootSpreadSeparationAngle = 5;
    [SerializeField] private int _amountOfProjectiles = 1;

    #endregion

    #region Unity Loops

    private void Awake()
    {
        _chipDataSO = GameAssetsManager.Instance.ChipDataSO;
    }

    public override void Start()
    {
        _weapon = GetComponentInParent<Weapon>();
        _chipClass = _chipDataSO.GetChipType(DamageType.Fire);
        _timeBetweenShots = _chipClass.ShootAfterSeconds;
    }

    #endregion

    public override void CheckShootInput()
    {
        if (_weapon.CurrentPlayer.IsUsing && !_hasAlreadyShot)
        {
            Shoot();
            _hasAlreadyShot = true;
        }
        else if (!_weapon.CurrentPlayer.IsUsing && _hasAlreadyShot)
        {
            _hasAlreadyShot = false;
        }
    }

    public override void Shoot()
    {
        if (_timeBetweenShots > _timeSinceLastShot) { return; }

        _timeSinceLastShot = 0f;

        ProjectileShootAngle();
    }

    private void ProjectileShootAngle()
    {
        //We first check if the amount of projectiles is odd or even, if it is odd, we'll instantiate a projectile in the middle of the cone
        if (_amountOfProjectiles % 2 == 1)
        {
            Instantiate(_weapon.ProjectilePrefab, _weapon.ShootTransforms[0].position, _weapon.TurretHead.rotation);
            ProjectileInstantiation(0);
        }
        else
        {
            //If it is even you need to instantiate 2 projectiles with the angle halved, since the separation angle only accounts for shots with the same separation angle
            //This means that the first two shots are going to have separation angle that is double, since the separation starts at the center of the barrel
            //This is not an issue with the odd, since the projectile of the middle makes it so that the angle of separation is correct
            Instantiate(_weapon.ProjectilePrefab, _weapon.ShootTransforms[0].position, _weapon.TurretHead.rotation * Quaternion.Euler(_shootSpreadSeparationAngle / 2, 0, 0));
            Instantiate(_weapon.ProjectilePrefab, _weapon.ShootTransforms[0].position, _weapon.TurretHead.rotation * Quaternion.Euler(-_shootSpreadSeparationAngle / 2, 0, 0));
            ProjectileInstantiation(1);
        }
    }

    private void ProjectileInstantiation(int isOddOrEven)
    {
        //If the number of projectiles is odd the variable needs to be 0, otherwise 1
        //It needs to be 1 or else, it will spawn unnecesary projectiles with an incorrect angle
        for (float i = (1 + isOddOrEven); i < _amountOfProjectiles; i += 2)
        {
            Instantiate(_weapon.ProjectilePrefab, _weapon.ShootTransforms[0].position, _weapon.TurretHead.rotation * Quaternion.Euler(_shootSpreadSeparationAngle * (i / 2 + 0.5f), 0, 0));
            Instantiate(_weapon.ProjectilePrefab, _weapon.ShootTransforms[0].position, _weapon.TurretHead.rotation * Quaternion.Euler(-_shootSpreadSeparationAngle * (i / 2 + 0.5f), 0, 0));
        }
    }
}