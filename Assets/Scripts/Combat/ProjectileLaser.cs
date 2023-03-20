using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLaser : Projectile
{
    #region Editor Fields
    
    [ColorUsageAttribute(false, true), SerializeField] private Color _laserHeatColor;

    #endregion

    public override void Start()
    {
        _destroyOnHit = false;

        base.Start();

        _dealDamageAfterSeconds = _damageData.AdditionalValue[0];

    }
}