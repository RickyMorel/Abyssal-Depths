using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

public class WeaponAttackHitBox : AttackHitBox
{
    #region Editors Field

    [SerializeField] protected Weapon _weapon;
    [SerializeField] private MeleeWeapon _meleeWeapon;
    [SerializeField] private float _timeBetweenDamages = -1f;

    #endregion

    #region Private Variables

    private float _timeSinceLastDamage;

    #endregion

    #region Public Properties

    public UnityEvent<Collider> OnHitSomething;

    #endregion

    #region Unity Loops

    public override void Start()
    {
        _damageData = DamageData.GetDamageData(_damageTypes, _weapon, -1);
    }

    private void Update()
    {
        _timeSinceLastDamage += Time.deltaTime;   
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
        if (DoesTriggerStayDamage()) { return; }

        Impact(collision.collider);
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (DoesTriggerStayDamage()) { return; }

        Impact(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!DoesTriggerStayDamage()) { return; }

        if(_timeSinceLastDamage < _timeBetweenDamages) { return; }

        _timeSinceLastDamage = 0f;

        Impact(other);
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

    private bool DoesTriggerStayDamage()
    {
        return _timeBetweenDamages > 0f;
    }
}
