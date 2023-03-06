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
        _dealDamageAfterSeconds = 0.5f;

        if (_weapon == null) { GameAssetsManager.Instance.EnemyDamageDataSO.CreateDamageForEnemies(_damageTypes, _aiCombatID, ref _damageData); }
        else 
        {
            GameAssetsManager.Instance.ChipDataSO.CreateDamageDataFromChip(_damageTypes, _weapon, ref _damageData);
            
            if (_particles.Length < 1) { return; }

            GameAssetsManager.Instance.ChipDataSO.ChangeParticleColor(_particles[0], _damageTypes[0], _weapon.ChipLevel);
        }
    }
}