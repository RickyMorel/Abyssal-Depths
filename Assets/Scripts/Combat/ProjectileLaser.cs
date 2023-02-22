using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLaser : Projectile
{
    public override void Start()
    {
        if (GetComponentInChildren<ParticleSystem>() == null) { return; }

        for (int i = 0; i < 2; i++)
        {
            _chipClass[i] = _chipDataSO.GetChipType(_damageTypes[i]);
            _chipDataSO.GetWeaknessAndResistance(_chipClass[i], out _weakness[i], out _resistance[i]);
            _damage[i] = _chipDataSO.GetDamageFromChip(_chipClass[i], _weapon.ChipLevel, 0);
            _secondaryValue[i] = _chipDataSO.GetDamageFromChip(_chipClass[i], _weapon.ChipLevel, 1);

            if (_damageTypes[i] == DamageType.Fire || _damageTypes[i] == DamageType.Laser) { _additionalValue[i] = _chipDataSO.GetAdditionalValueFromChip(_chipClass[i]); }
        }

        _impactDamage = _chipDataSO.GetImpactDamageFromChip(_chipClass[0]);

        if (_damageTypes[0] == _damageTypes[1]) { _chipDataSO.GetBonusFromChip(_chipClass[0], ref _damage[0], ref _secondaryValue[0], ref _additionalValue[0]); }

        _particles = GetComponentInChildren<ParticleSystem>();
        _destroyOnHit = false;
        _dealDamageAfterSeconds = 0.5f;
    }
}