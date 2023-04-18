using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class AttackHitBox : MonoBehaviour
{
    #region Public Properties

    public event Action<GameObject> OnHit;

    #endregion

    #region Private Variable

    protected int _damage;
    protected DamageData _damageData;
    private int _aiCombatID = -1;

    #endregion

    #region Editor Fields

    [Tooltip(" Selects a stat from EnemyDamageDataSO:\n 0-Damage element 0\n 1-Damage element 1\n 2-SecondaryValue element 0\n 3-SecondaryValue element 1\n")]
    [SerializeField] private int _enemyDamageStat = -1;
    [SerializeField] protected Damageable _ownHealth;
    [SerializeField] protected DamageTypes[] _damageTypes;

    #endregion

    #region Getters And Setters

    public int AICombatID { get { return _aiCombatID; } set { _aiCombatID = value; } }

    #endregion

    #region Unity Loops

    public virtual void Start()
    {
        if (_aiCombatID < 0)
        {
            DamageTypes[] damageTypes = { DamageTypes.None, DamageTypes.None };
            _damageData = DamageData.GetDamageData(damageTypes, null, -1);
        }
        else
        {
            _damageData = DamageData.GetDamageData(_damageTypes, null, _aiCombatID);

        }
    }

    #endregion

    public virtual void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Damageable enemyHealth)) { return; }

        if (_ownHealth != null && enemyHealth == _ownHealth) { return; }

        DealDamageToPlayerOrShip(enemyHealth);
    }

    public void InvokeHitParticles(Collider other)
    {
        OnHit?.Invoke(other.gameObject);
    }

    public void DealDamageToEnemies(AIHealth enemyHealth)
    {
        AIHealth aiHealth = enemyHealth;
        if (aiHealth.CanKill)
        {
            enemyHealth.DamageData = _damageData;
            enemyHealth.Damage(_damage);
        }
        else { aiHealth.Hurt(DamageTypes.Base, 0); }

        _ownHealth.GetComponent<PlayerComponents>()?.PlayerCamera.NormalShake();
    }

    public void DealDamageToPlayerOrShip(Damageable enemyHealth)
    {
        if (enemyHealth is PlayerHealth)
        {
            PlayerHealth playerHealth = enemyHealth as PlayerHealth;
            playerHealth.Hurt(DamageTypes.Base, 0);
            _ownHealth.GetComponent<PlayerComponents>()?.PlayerCamera.NormalShake();
        }

        if (enemyHealth is ShipHealth)
        {
            enemyHealth.DamageData = _damageData;

            int damage = 0;

            switch (_enemyDamageStat)
            {
                case 0|-1:
                    damage = _damageData.Damage[0];
                    break;
                case 1:
                    damage = _damageData.Damage[1];
                    break;
                case 2:
                    damage = (int)_damageData.SecondaryValue[0];
                    break;
                case 3:
                    damage = (int)_damageData.SecondaryValue[1];
                    break;
            }

            enemyHealth.Damage(damage);
        }
    }
}