using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaceAttackHitBox : AttackHitBox
{
    #region Editors Field

    [SerializeField] private Weapon _weapon;
    [SerializeField] private MeleeWeapon _meleeWeapon;

    #endregion

    #region Unity Loops

    public override void Start()
    {
        _damageData = DamageData.GetDamageData(_damageTypes, _weapon, -1);
    }

    #endregion

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6) { InvokeHitParticles(other); }

        if (!other.gameObject.TryGetComponent(out Damageable enemyHealth)) { return; }

        if (_ownHealth != null && enemyHealth == _ownHealth) { return; }

        InvokeHitParticles(other);

        if (enemyHealth is AIHealth) 
        {
            CalculateDamage();

            DealDamageToEnemies((AIHealth)enemyHealth);
        }
    }

    private void CalculateDamage()
    {
        _damage = (int)_damageData.SecondaryValue[0];
        if (Mathf.Abs(_meleeWeapon.rb.velocity.x) >= (_meleeWeapon.MaxMovementSpeed / 2) || Mathf.Abs(_meleeWeapon.rb.velocity.y) >= (_meleeWeapon.MaxMovementSpeed / 2))
        {
            float damagePercentage;
            if (Mathf.Abs(_meleeWeapon.rb.velocity.x) >= Mathf.Abs(_meleeWeapon.rb.velocity.y))
            {
                damagePercentage = (Mathf.Abs(_meleeWeapon.rb.velocity.x) * 100) / _meleeWeapon.MaxMovementSpeed;
            }
            else
            {
                damagePercentage = (Mathf.Abs(_meleeWeapon.rb.velocity.y) * 100) / _meleeWeapon.MaxMovementSpeed;
            }
            _damage = (int)((_damage * damagePercentage) / 100);
        }
        else { _damage = _damageData.Damage[0]; }
    }
}