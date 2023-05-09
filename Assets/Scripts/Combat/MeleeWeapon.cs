using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeleeWeapon : WeaponShoot
{
    #region Editor Fields

    [SerializeField] protected AttackHitBox _attackHitBox;
    [SerializeField] protected Transform _parentTransform;
    [SerializeField] protected float maxDistance = 10f;
    [SerializeField] protected float _moveForce = 50f;
    [SerializeField] protected float _maxMovementSpeed = 20f;

    #endregion

    #region Private Variable

    protected Rigidbody _rb;

    #endregion

    #region Public Properties

    public float MaxMovementSpeed => _maxMovementSpeed;
    public Rigidbody rb => _rb;

    #endregion

    #region Unity Loops

    public virtual void Awake()
    {
        _rb = _attackHitBox.GetComponent<Rigidbody>();

        _attackHitBox.OnHit += HandleHitParticles;
    }

    public virtual void OnDestroy()
    {
        _attackHitBox.OnHit -= HandleHitParticles;
    }

    public virtual void OnEnable()
    {
        //For child classes
    }

    public virtual void OnDisable()
    {
        //For child classes
    }

    public virtual void FixedUpdate()
    {
        if (_weapon.CurrentPlayer == null) { return; }
        if (_weapon.CanUse == false) { return; }

        _rb.AddForce(_weapon.CurrentPlayer.MoveDirection * _moveForce);
        _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, _maxMovementSpeed);
    }

    #endregion

    public override void CheckShootInput()
    {
        //do nothing
    }

    public override void Shoot()
    {
        //do nothing
    }

    public virtual void HandleHitParticles(GameObject obj)
    {
        if (obj.tag == "MainShip") { return; }

        Instantiate(GameAssetsManager.Instance.MeleeFloorHitParticles, _attackHitBox.transform.position, Quaternion.identity);
        ShipCamera.Instance.ShakeCamera(5f, 50f, 0.2f);
    }
}