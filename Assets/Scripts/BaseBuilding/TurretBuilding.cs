using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurretBuilding : BuildingUpgradable
{
    #region Editor Fields

    [Header("Prefabs")]
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private GameObject _projectileShellPrefab;

    [Header("Gameobject Vars")]
    [SerializeField] private Transform _shootTransform;
    [SerializeField] private Transform _turretHeadTransform;

    [Header("Stats")]
    [Range(100, 199)]
    [SerializeField] private int _combatId = 100;
    [SerializeField] private float _timeBetweenShots;

    [Header("Sounds")]
    [SerializeField] private EventReference _shootingSfx;

    #endregion

    #region Private Varaibles

    private AIHealth _currentTarget;
    private Queue<AIHealth> _potentialTargets = new Queue<AIHealth>();
    private ParticleSystem _shootBubbleParticles;
    private float _timeSinceLastShot = 999f;
    private float _timeSinceLastTarget = 999f;
    private float _timeUntilTargetLock = 0.6f;
    private Vector3 _originalWeaponHeadPosition;

    #endregion

    #region Unity Loops

    public override void Start()
    {
        base.Start();

        _originalWeaponHeadPosition = _turretHeadTransform.transform.localPosition;
        _shootBubbleParticles = WeaponFX.InstantiateBubbleParticles(_shootTransform);
    }

    private void Update()
    {
        _timeSinceLastShot += Time.deltaTime;
        _timeSinceLastTarget += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (!CanUse()) { return; }

        if (_currentTarget == null || _currentTarget.IsDead()) { TargetEnemy(); return; }

        LookAtTarget();

        Shoot();
    }

    public void OnEnemyZoneEnter(Collider other)
    {
        if(!other.gameObject.TryGetComponent(out AIHealth aIHealth)) { return; }

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
        var rotation = Quaternion.LookRotation(lookPos);
        _turretHeadTransform.rotation = Quaternion.Slerp(_turretHeadTransform.rotation, rotation, Time.deltaTime * 20f);
    }

    private void Shoot()
    {
        if (_timeSinceLastTarget < _timeUntilTargetLock) { return; }

        if (_timeSinceLastShot < _timeBetweenShots) { return; }

        _timeSinceLastShot = 0f;

        Projectile newProjectile = Instantiate(_projectilePrefab.gameObject, _shootTransform.position, _shootTransform.rotation).GetComponent<Projectile>();
        newProjectile.Initialize(tag, transform);
        newProjectile.AICombatID = _combatId;

        WeaponFX.PlayShootFX(this, _shootTransform, _turretHeadTransform, _timeBetweenShots, _originalWeaponHeadPosition, _projectileShellPrefab, _shootingSfx, _shootBubbleParticles);
    }
}
