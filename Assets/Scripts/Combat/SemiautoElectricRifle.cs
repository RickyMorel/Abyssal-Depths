using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemiautoElectricRifle : WeaponShoot
{
    #region Private Variable

    private bool _hasAlreadyShot = false;
    private ChipDataSO _chipDataSO;
    private ChipDataSO.BasicChip _chipClass;

    #endregion

    #region Unity Loops

    private void Awake()
    {
        _chipDataSO = GameAssetsManager.Instance.ChipDataSO;
    }

    public override void Start()
    {
        _weapon = GetComponentInParent<Weapon>();
        _chipClass = _chipDataSO.GetChipType(DamageType.Electric);
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
}