using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mace : WeaponShoot
{
    #region Editor Fields

    [SerializeField] private GameObject _maceHead;
    [SerializeField] private Transform _parentTransform;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float _moveForce = 50f;
    [SerializeField] private float _maxMovementSpeed = 20f;

    #endregion

    #region Private Variable

    private Rigidbody _rb;
    private AttackHitBox _attackHitBox;

    #endregion

    #region Public Properties

    public float MaxMovementSpeed => _maxMovementSpeed;
    public Rigidbody rb => _rb;

    #endregion

    private void Awake()
    {
        _rb = _maceHead.GetComponent<Rigidbody>();
        _attackHitBox = _maceHead.GetComponent<AttackHitBox>();

        _attackHitBox.OnHit += HandleHitParticles;
    }

    private void OnDestroy()
    {
        _attackHitBox.OnHit -= HandleHitParticles;
    }

    private void OnEnable()
    {
        _maceHead.transform.parent = null;
    }

    private void OnDisable()
    {
        if(_parentTransform == null) { return; }

        _maceHead.transform.parent = _parentTransform;
    }

    private void FixedUpdate()
    {
        if (_weapon.CurrentPlayer == null) { return; }
        if(_weapon.CanUse == false) { return; }

        _rb.AddForce(_weapon.CurrentPlayer.MoveDirection * _moveForce);
        _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, _maxMovementSpeed);
    }

    public override void CheckShootInput()
    {
        //do nothing
    }

    public override void Shoot()
    {
        //do nothing
    }

    private void HandleHitParticles(GameObject obj)
    {
        if(obj.tag == "MainShip") { return; }

        Instantiate(GameAssetsManager.Instance.MeleeFloorHitParticles, _maceHead.transform.position, Quaternion.identity);
        ShipCamera.Instance.ShakeCamera(5f, 50f, 0.2f);
    }
}