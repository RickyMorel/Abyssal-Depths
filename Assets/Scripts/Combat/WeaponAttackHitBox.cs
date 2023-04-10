using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttackHitBox : AttackHitBox
{
    #region Editors Field

    [SerializeField] protected Weapon _weapon;

    #endregion

    public virtual void Start()
    {
        _damageData = DamageData.GetDamageData(_damageTypes, _weapon, -1);
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Damageable enemyHealth)) { return; }

        if (enemyHealth is AIHealth)
        {
            DealDamageToEnemies((AIHealth)enemyHealth, _damageData.Damage[0], _damageData.Damage[1]);
        }
    }
}