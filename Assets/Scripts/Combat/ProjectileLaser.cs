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
        if (_weapon == null) { CreateDamageForEnemies(); }
        else { CreateDamageDataFromChip(); }

        _destroyOnHit = false;
        _dealDamageAfterSeconds = 0.5f;
    }
}