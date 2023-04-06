using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaberAttackHitBox : MaceAttackHitBox
{
    #region Private Variables

    private int _secondaryDamage;

    #endregion

    public override void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Damageable enemyHealth)) { return; }

        if (enemyHealth is AIHealth)
        {
            CalculateDamage();

            DealDamageToEnemies((AIHealth)enemyHealth, _damage, _secondaryDamage);
        }
    }

    public override void CalculateDamage()
    {
        _damage = _damageData.Damage[0];
        _secondaryDamage = _damageData.Damage[1];
    }
}