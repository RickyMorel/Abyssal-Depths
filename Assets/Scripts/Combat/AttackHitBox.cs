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

    protected void CheckForParticles(Collider other)
    {
        if (other.gameObject.layer == 6) { OnHit?.Invoke(other.gameObject); }

        OnHit?.Invoke(other.gameObject);
    }

    protected void DealDamageToEnemies(AIHealth enemyHealth)
    {
        AIHealth aiHealth = enemyHealth;
        if (aiHealth.CanKill)
        {
            enemyHealth.DamageData = _damageData;
            enemyHealth.Damage(_damage);
        }
        else { aiHealth.Hurt(DamageTypes.Base); }

        _ownHealth.GetComponent<PlayerComponents>()?.PlayerCamera.ShakeCamera(2f, 50f, 0.2f);
    }

    protected void DealDamageToPlayerOrShip(Damageable enemyHealth)
    {
        if (enemyHealth is PlayerHealth)
        {
            PlayerHealth playerHealth = enemyHealth as PlayerHealth;
            playerHealth.Hurt(DamageTypes.Base);
            _ownHealth.GetComponent<PlayerComponents>()?.PlayerCamera.ShakeCamera(2f, 50f, 0.2f);
        }

        if (enemyHealth is ShipHealth)
        {
            enemyHealth.DamageData = _damageData;
            enemyHealth.Damage(_damageData.Damage[0]);
        }
    }
}