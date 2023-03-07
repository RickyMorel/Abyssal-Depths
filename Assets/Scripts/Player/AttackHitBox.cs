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

    private Mace _mace;
    private ChipDataSO.BasicChip _chipClass;
    private ChipDataSO _chipDataSO;
    private int _damage;
    private Weapon _weapon;
    private DamageData _damageData;
    private int _aiCombatID;
    public int AICombatID { get { return _aiCombatID; } set { _aiCombatID = value; } }

    #endregion

    #region Editor Fields

    [SerializeField] private Damageable _ownHealth;
    [SerializeField] private bool _isFriendlyToPlayers = true;
    [SerializeField] private DamageTypes[] _damageTypes;

    #endregion

    #region Unity Loops

    private void Start()
    {
        _mace = GetComponentInParent<Mace>();

        if (_weapon == null) { GameAssetsManager.Instance.EnemyDamageDataSO.CreateDamageForEnemies(_damageTypes, _aiCombatID, ref _damageData); }
        else
        {
            GameAssetsManager.Instance.ChipDataSO.CreateDamageDataFromChip(_damageTypes, _weapon, ref _damageData);
        }
    }

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 6) { OnHit?.Invoke(other.gameObject); }

        if (!other.gameObject.TryGetComponent(out Damageable enemyHealth)) { return; }

        if (_ownHealth != null && enemyHealth == _ownHealth) { return; }

        OnHit?.Invoke(other.gameObject);

        if (enemyHealth is AIHealth)
        {
            AIHealth aiHealth = (AIHealth)enemyHealth;
            if (aiHealth.CanKill) 
            {
                CalculateDamage(enemyHealth);
            }
            else { aiHealth.Hurt(DamageTypes.Base); }

            _ownHealth.GetComponent<PlayerComponents>()?.PlayerCamera.ShakeCamera(2f, 50f, 0.2f);
        }

        if (_isFriendlyToPlayers) { return; }

        if (enemyHealth is PlayerHealth) 
        { 
            PlayerHealth playerHealth = enemyHealth as PlayerHealth;
            playerHealth.Hurt(DamageTypes.Base);
            _ownHealth.GetComponent<PlayerComponents>()?.PlayerCamera.ShakeCamera(2f, 50f, 0.2f);
        }

        if (enemyHealth is ShipHealth)
        {
            CalculateDamage(enemyHealth);
        }
    }

    private void CalculateDamage(Damageable health)
    {
        if (_mace.rb.velocity.x <= _mace.MaxMovementSpeed / 2 && _mace.rb.velocity.y < _mace.MaxMovementSpeed / 2) { _damage = _chipDataSO.GetDamageFromChip(_chipClass, _weapon.ChipLevel, 0); health.Damage(_damage); }
        else { _damage = _chipDataSO.GetDamageFromChip(_chipClass, _weapon.ChipLevel, 1); health.Damage(_damage); }
    }
}