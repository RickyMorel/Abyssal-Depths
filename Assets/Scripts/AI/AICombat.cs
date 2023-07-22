using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICombat : PlayerCombat
{
    #region Editor Fields

    [SerializeField] private Transform _projectileHolsterTransform;
    [SerializeField] private int _enemyDamageDataID;
    [Range(2f, 100f)]
    [SerializeField] private float _attackRange = 10f;

    [SerializeField] private ParticleSystem _hitParticle;

    #endregion

    #region Public Properties

    public float AttackRange => _attackRange;

    #endregion

    private GAgent _gAgent;
    private EnemyPushAttackCollider _enemyPushAttackCollider;

    private void Awake()
    {
        if (_attackHitbox == null) { return; }

        _attackHitbox.GetComponent<AttackHitBox>().AICombatID = _enemyDamageDataID;
    }

    private void Start()
    {
        _gAgent = GetComponent<GAgent>();
        _enemyPushAttackCollider = GetComponentInChildren<EnemyPushAttackCollider>();  
    }

    public override void Hit()
    {
        base.Hit();

        _hitParticle?.Play();
    }

    public override void Shoot()
    {
        if(_gAgent.CurrentAction == null) { return; }

        DestroyPrevHeldProjectile();
        Transform enemyTransform = _gAgent.CurrentAction.Target.transform;
        GameObject newProjectile = Instantiate(_projectilePrefab, _shootTransform.position, _shootTransform.rotation);
        newProjectile.transform.LookAt(enemyTransform);
        Projectile projectile = newProjectile.GetComponent<Projectile>();
        projectile.Initialize(tag, transform);
        projectile.AICombatID = _enemyDamageDataID;

        _gAgent.StateMachine.AIAudio.PlayShootSFX();
    }

    public void EnablePushAttack()
    {
        if(_enemyPushAttackCollider == null) { return; }

        _enemyPushAttackCollider.PushAttack();
    }

    public void HoldProjectileInHolster()
    {
        DestroyPrevHeldProjectile();

        GameObject projectileMesh = _projectilePrefab.transform.Find("Mesh").gameObject;

        GameObject projectileMeshInstance = Instantiate(projectileMesh, _projectileHolsterTransform);
        projectileMeshInstance.transform.localPosition = Vector3.zero;
        projectileMeshInstance.transform.localRotation = Quaternion.identity;
    }

    public void DestroyPrevHeldProjectile()
    {
        foreach (Transform child in _projectileHolsterTransform)
        {
            if(child == _projectileHolsterTransform) { continue; }

            Destroy(child.gameObject);
        }
    }

    public void Aggro()
    {
        if (_gAgent == null) { return; }

        if (_gAgent.Beliefs.HasState("aggro")) { return; }

        _gAgent.Beliefs.AddState("aggro", 1);

        _gAgent.StateMachine.AIAudio.PlayShootSFX();
    }
}