using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponAttackHitBox : AttackHitBox
{
    #region Editors Field

    [SerializeField] protected Weapon _weapon;
    [SerializeField] private MeleeWeapon _meleeWeapon;
    [SerializeField] private bool _doesOnTriggerStayDamage = false;

    #endregion

    #region Public Properties

    public UnityEvent<Collider> OnHitSomething;

    #endregion

    #region Unity Loops

    public override void Start()
    {
        _damageData = DamageData.GetDamageData(_damageTypes, _weapon, -1);
    }

    #endregion

    public void Initialize(Weapon weapon, Damageable ownHealth, MeleeWeapon meleeWeapon)
    {
        _weapon = weapon;
        _ownHealth = ownHealth;
        _meleeWeapon = meleeWeapon;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_doesOnTriggerStayDamage) { return; }

        Impact(collision.collider);
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (_doesOnTriggerStayDamage) { return; }

        Impact(other);
    }

    private void OnTriggerStay(Collider other)
    {
        //Ask Ariel where I can find time between electrocutions and what is StunRaduis used for in ChipDataSO
        if (!_doesOnTriggerStayDamage) { return; }

        //Impact(other);
    }

    private void Impact(Collider other)
    {
        OnHitSomething?.Invoke(other);

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
