using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLaser : Projectile
{
    public override void Start()
    {
        if (GetComponentInChildren<ParticleSystem>() == null) { return; }

        _chipClass = _chipDataSO.GetChipType(_damageType);
        _chipDataSO.GetWeaknessAndResistance(_chipClass, out _weakness, out _resistance);
        _damage = _chipDataSO.GetDamageFromChip(_chipClass, _weapon.ChipLevel);
        _secondaryValue = _chipDataSO.GetSecondaryValueFromChip(_chipClass, _weapon.ChipLevel);

        if (_damageType == DamageType.Electric || _damageType == DamageType.Fire) { _impactDamage = _chipDataSO.GetImpactDamageFromChip(_chipClass); }
        else if (_damageType == DamageType.Fire || _damageType == DamageType.Laser) { _additionalValue = _chipDataSO.GetAdditionalValueFromChip(_chipClass); }

        _particles = GetComponentInChildren<ParticleSystem>();
        _destroyOnHit = false;
        _dealDamageAfterSeconds = 0.5f;
    }
}