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
    private Collider _hitBox;

    #endregion

    #region Editor Fields

    [SerializeField] private Damageable _ownHealth;
    [SerializeField] private bool _isFriendlyToPlayers = true;

    #endregion

    private void Start()
    {
        _hitBox = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 6) { OnHit?.Invoke(other.gameObject); }

        if (!other.gameObject.TryGetComponent<Damageable>(out Damageable enemyHealth)) { return; }

        if (_ownHealth != null && enemyHealth == _ownHealth) { return; }

        OnHit?.Invoke(other.gameObject);

        if (enemyHealth is AIHealth)
        {
            AIHealth aiHealth = (AIHealth)enemyHealth;
            if (aiHealth.CanKill) { enemyHealth.Damage(20, DamageType.Base); }
            else { aiHealth.Hurt(DamageType.Base); }

            _ownHealth.GetComponent<PlayerComponents>()?.PlayerCamera.ShakeCamera(2f, 50f, 0.2f);
        }

        if (_isFriendlyToPlayers) { return; }

        if (enemyHealth is PlayerHealth) 
        { 
            PlayerHealth playerHealth = enemyHealth as PlayerHealth;
            playerHealth.Hurt(DamageType.Base);
            _ownHealth.GetComponent<PlayerComponents>()?.PlayerCamera.ShakeCamera(2f, 50f, 0.2f);
        }

        if (enemyHealth is ShipHealth)
        {
            enemyHealth.Damage(20, DamageType.Base, false, _hitBox);
        }
    }
}