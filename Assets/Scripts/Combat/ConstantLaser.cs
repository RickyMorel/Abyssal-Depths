using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantLaser : WeaponShoot
{
    #region Unity Loops

    public override void Start()
    {
        base.Start();

        _weapon.ProjectilePrefab.SetActive(false);
    }

    #endregion


    public override void CheckShootInput()
    {
        if (_weapon.CurrentPlayer.IsUsing && !_weapon.ProjectilePrefab.activeSelf)
        {
            _weapon.ProjectilePrefab.SetActive(true);
            Debug.Log(_weapon.CurrentPlayer);
        }
        else if (!_weapon.CurrentPlayer.IsUsing && _weapon.ProjectilePrefab.activeSelf)
        {
            _weapon.ProjectilePrefab.SetActive(false);
        }

    }
}