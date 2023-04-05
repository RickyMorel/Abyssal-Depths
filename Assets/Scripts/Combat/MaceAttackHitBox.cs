using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaceAttackHitBox : AttackHitBox
{
    #region Editors Field

    [SerializeField] private Weapon _weapon;
    [SerializeField] private Mace _mace;

    #endregion

    #region Unity Loops

    public override void Start()
    {
        _damageData = DamageData.GetDamageData(_damageTypes, _weapon, -1);
    }

    #endregion

    public override void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Damageable enemyHealth)) { return; }

        if (_ownHealth != null && enemyHealth == _ownHealth) { return; }

        if(enemyHealth is AIHealth) 
        {
            InvokeHitParticles(other);

            CalculateDamage();

            DealDamageToEnemies((AIHealth)enemyHealth);
        }
    }

    private void CalculateDamage()
    {
        _damage = (int)_damageData.SecondaryValue[0];
        if (Mathf.Abs(_mace.rb.velocity.x) >= (_mace.MaxMovementSpeed / 2) || Mathf.Abs(_mace.rb.velocity.y) >= (_mace.MaxMovementSpeed / 2))
        {
            float damagePercentage;
            if (Mathf.Abs(_mace.rb.velocity.x) >= Mathf.Abs(_mace.rb.velocity.y))
            {
                damagePercentage = (Mathf.Abs(_mace.rb.velocity.x) * 100) / _mace.MaxMovementSpeed;
            }
            else
            {
                damagePercentage = (Mathf.Abs(_mace.rb.velocity.y) * 100) / _mace.MaxMovementSpeed;
            }
            _damage = (int)((_damage * damagePercentage) / 100);
        }
        else { _damage = _damageData.Damage[0]; }
    }
}