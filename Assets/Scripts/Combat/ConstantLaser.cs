using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantLaser : WeaponShoot
{
    #region Private Variables

    private bool _isShooting = false;
    private GameObject _laserBeam;

    #endregion

    public override void CheckShootInput()
    {
        if (_weapon.CurrentPlayer.IsUsing)
        {
            Shoot();
        }
        else 
        {
            _laserBeam.transform.SetParent(null);
            Destroy(_laserBeam);
            _isShooting = false;
        }
    }

    public override void Shoot()
    {
        if (!_isShooting) 
        { 
            _laserBeam = Instantiate(_weapon.ProjectilePrefab, _weapon.ShootTransforms[0].position, _weapon.TurretHead.rotation);
            _laserBeam.transform.SetParent(_weapon.TurretHead);
            _isShooting = true;
        }
        else { return; }
    }

}