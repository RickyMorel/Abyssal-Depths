using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttackHitBox : AttackHitBox
{
    #region Editors Field

    [SerializeField] protected Weapon _weapon;
    [SerializeField] private MeleeWeapon _meleeWeapon;

    #endregion

    #region Unity Loops

    public override void Start()
    {
        _damageData = DamageData.GetDamageData(_damageTypes, _weapon, -1);
    }

    #endregion

    //layer 6 is floor
    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6) { InvokeHitParticles(other); }

        if (!other.gameObject.TryGetComponent(out Damageable enemyHealth)) { return; }

        if (_ownHealth != null && enemyHealth == _ownHealth) { return; }

        InvokeHitParticles(other);

        if (enemyHealth is AIHealth)
        {
            DealDamageToEnemies((AIHealth)enemyHealth, (_damageData.Damage[0]), (_damageData.Damage[1]));
        }
    }
}
