using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICombat : PlayerCombat
{
    #region Editor Fields

    [SerializeField] private int _enemyDamageDataID;
    [Range(2f, 100f)]
    [SerializeField] private float _attackRange = 10f;

    #endregion

    #region Public Properties

    public float AttackRange => _attackRange;

    #endregion

    private GAgent _gAgent;

    private void Awake()
    {
        if (_attackHitbox.GetComponents<AttackHitBox>() != null)
        {
            _attackHitbox.GetComponent<AttackHitBox>().AICombatID = _enemyDamageDataID;
        }
    }

    private void Start()
    {
        _gAgent = GetComponent<GAgent>();
    }

    public override void Shoot()
    {
        if(_gAgent.CurrentAction == null) { return; }

        Transform enemyTransform = _gAgent.CurrentAction.Target.transform;
        GameObject newProjectile = Instantiate(_projectilePrefab, _shootTransform.position, _shootTransform.rotation);
        newProjectile.transform.LookAt(enemyTransform);
        newProjectile.GetComponent<Projectile>().Initialize(tag);
        newProjectile.GetComponent<Projectile>().AICombatID = _enemyDamageDataID;
    }

    public void Aggro()
    {
        if (_gAgent == null) { return; }

        if (_gAgent.Beliefs.HasState("aggro")) { return; }

        _gAgent.Beliefs.AddState("aggro", 1);
    }
}