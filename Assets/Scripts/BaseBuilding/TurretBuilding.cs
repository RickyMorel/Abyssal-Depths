using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurretBuilding : BuildingUpgradable
{
    #region Editor Fields

    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private Transform _shootTransform;
    [SerializeField] private Transform _turretHeadTransform;
    [Range(100, 199)]
    [SerializeField] private int _combatId = 100;
    [SerializeField] private float _timeBetweenShots;

    #endregion

    #region Private Varaibles

    private AIHealth _currentTarget;
    private Queue<AIHealth> _potentialTargets = new Queue<AIHealth>();
    private float _timeSinceLastShot = 999f;
    private float _timeSinceLastTarget = 999f;
    private float _timeUntilTargetLock = 0.6f;

    #endregion

    #region Unity Loops

    private void Update()
    {
        _timeSinceLastShot += Time.deltaTime;
        _timeSinceLastTarget += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (_currentTarget == null || _currentTarget.IsDead()) { TargetEnemy(); return; }

        LookAtTarget();

        Shoot();
    }

    public void OnEnemyZoneEnter(Collider other)
    {
        Debug.Log("OnEnemyZoneEnter: " + other.gameObject.name);

        if(!other.gameObject.TryGetComponent(out AIHealth aIHealth)) { return; }

        Debug.Log("OnEnemyZoneEnter got enemy!");

        _potentialTargets.Enqueue(aIHealth);
    }

    public void OnEnemyZoneExit(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out AIHealth aIHealth)) { return; }
        
        _potentialTargets = new Queue<AIHealth>(_potentialTargets.Where(x => x != aIHealth));
    }

    #endregion

    private void TargetEnemy()
    {
        if (_currentTarget != null && !_currentTarget.IsDead()) { return; }

        if(_potentialTargets.Count == 0) { return; }

        Queue<AIHealth> copiedQueue = new Queue<AIHealth>(_potentialTargets);

        AIHealth wantedEnemy = copiedQueue.Dequeue();

        _currentTarget = wantedEnemy;

        _timeSinceLastTarget = 0f;
    }

    private void LookAtTarget()
    {
        var lookPos = _currentTarget.transform.position - _turretHeadTransform.position;
        //lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        _turretHeadTransform.rotation = Quaternion.Slerp(_turretHeadTransform.rotation, rotation, Time.deltaTime * 20f);
    }

    private void Shoot()
    {
        if (_timeSinceLastTarget < _timeUntilTargetLock) { return; }

        if (_timeSinceLastShot < _timeBetweenShots) { return; }

        _timeSinceLastShot = 0f;

        //_anim.SetTrigger("Shoot");

        Debug.Log("Turret Shoot!");

        Projectile newProjectile = Instantiate(_projectilePrefab.gameObject, _shootTransform.position, _shootTransform.rotation).GetComponent<Projectile>();
        newProjectile.Initialize(tag, transform);
        newProjectile.AICombatID = _combatId;
    }
}
