using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttackHitBox : AttackHitBox
{
    #region Editors Field

    [SerializeField] protected Weapon _weapon;
    [SerializeField] private float _damageMultiplierForBladeThrow;

    #endregion

    #region Private Variables

    private LightSaber _lightSaber;

    #endregion

    public override void Start()
    {
        _lightSaber = GetComponentInParent<LightSaber>();
        _damageData = DamageData.GetDamageData(_damageTypes, _weapon, -1);
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Damageable enemyHealth)) { return; }

        if (enemyHealth is AIHealth)
        {
            if (!_lightSaber.IsBladeOut) { return; }

            if (_lightSaber.BoomerangThrow) { DealDamageToEnemies((AIHealth)enemyHealth, (int)(_damageData.Damage[0]  * _damageMultiplierForBladeThrow), (int)(_damageData.Damage[1] * _damageMultiplierForBladeThrow)); }
            else { DealDamageToEnemies((AIHealth)enemyHealth, _damageData.Damage[0], _damageData.Damage[1]); }
        }
    }
}