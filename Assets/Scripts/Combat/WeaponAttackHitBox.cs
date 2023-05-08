using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponAttackHitBox : AttackHitBox
{
    #region Editors Field

    [SerializeField] protected Weapon _weapon;
    [SerializeField] private MeleeWeapon _meleeWeapon;

    #endregion

    #region Public Properties

    public UnityEvent OnHitSomething;

    #endregion

    #region Unity Loops

    public override void Start()
    {
        _damageData = DamageData.GetDamageData(_damageTypes, _weapon, -1);
    }

    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        Impact(collision.collider);
    }

    public override void OnTriggerEnter(Collider other)
    {
        Impact(other);
    }

    private void Impact(Collider other)
    {
        OnHitSomething?.Invoke();

        //layer 6 is floor
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
