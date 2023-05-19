using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mace : MeleeWeapon
{
    #region Editor Fields

    [Header("GameObject Related")]
    [SerializeField] private GameObject _maceHead;
    [SerializeField] private Transform _handleTransform;
    [SerializeField] private Transform _moveToTransform;
    [Header("Floats")]
    [SerializeField] private float _grappleSpeed;
    [SerializeField] private float _flyingSpeed;

    #endregion

    #region Private Variables

    private ThrowState _throwState = ThrowState.Attached;
    private Rigidbody _maceRb;
    private bool _prevInputState;
    private LineRenderer _lr;
    private float _timePassedReturning;

    #endregion

    #region Unity Loops

    public override void OnEnable()
    {
        //do nothing
    }

    public override void OnDisable()
    {
        //do nothing
    }

    public override void Start()
    {
        base.Start();

        _maceRb = _maceHead.GetComponent<Rigidbody>();
        _lr = GetComponent<LineRenderer>();
        _lr.positionCount = 2;
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        //ThrowHarpoon();
    }

    #endregion

    public override void HandleHitParticles(GameObject obj)
    {
        if(obj.tag == "MainShip") { return; }

        Instantiate(GameAssetsManager.Instance.MeleeFloorHitParticles, _maceHead.transform.position, Quaternion.identity);
        ShipCamera.Instance.ShakeCamera(5f, 50f, 0.2f);
    }

    #region Shoot Functions

    public override void CheckShootInput()
    {
        //Shoots harpoon
        if (_weapon.CurrentPlayer.IsUsing == _prevInputState) { return; }

        _prevInputState = _weapon.CurrentPlayer.IsUsing;

        if (_weapon.CurrentPlayer.IsUsing)
        {
            Shoot();
        }
    }

    private void DrawRope()
    {
        _lr.SetPosition(0, _maceRb.transform.position);
        _lr.SetPosition(1, _handleTransform.position);
    }

    public override void Shoot()
    {
        if (_throwState != ThrowState.Attached)
        {
            if (_throwState == ThrowState.Arrived || _throwState == ThrowState.Stuck || _throwState == ThrowState.GrabbingEnemy) { ReturnHarpoon(); }

            return;
        }

        _throwState = ThrowState.Throwing;
        _maceRb.isKinematic = false;
        _weapon.ShouldRotate = false;

        _maceRb.transform.SetParent(null);
    }

    private void ThrowHarpoon()
    {
        if (_throwState == ThrowState.Arrived) { return; }

        Transform moveToCurrentPosition = _moveToTransform;

        if (_throwState == ThrowState.Throwing)
        {
            Vector3 forceDir = _moveToTransform.position - _maceRb.transform.position;
            _maceRb.AddForce(forceDir.normalized * _flyingSpeed, ForceMode.Force);
        }

        if (_throwState == ThrowState.Throwing && Vector3.Distance(_maceRb.transform.position, moveToCurrentPosition.position) < 2f)
        {
            _throwState = ThrowState.Arrived;
        }

        ReturnHarpoonToWeapon();
    }

    private void ReturnHarpoonToWeapon()
    {
        if (_maceRb.transform.position != _handleTransform.position && _throwState == ThrowState.Returning)
        {
            _timePassedReturning += Time.deltaTime;
            _maceRb.transform.position = Vector3.MoveTowards(_maceRb.transform.position, _handleTransform.position, Time.deltaTime * (_flyingSpeed / 10f) * _timePassedReturning);
        }
        if (_maceRb.transform.position == _handleTransform.position && _throwState == ThrowState.Returning)
        {
            _maceRb.transform.SetParent(_handleTransform);
            _maceRb.transform.localPosition = _handleTransform.localPosition;
            _maceRb.transform.localRotation = _handleTransform.localRotation;
            _weapon.ShouldRotate = true;
            _throwState = ThrowState.Attached;
            _timePassedReturning = 0f;
        }
    }

    private void ReturnHarpoon()
    {
        _throwState = ThrowState.Returning;
        //Destroy(_tetherSpringInstance);

        //if (_tetheredEnemy == null) { return; }

        //_tetheredEnemy.SetRagdollState(false);
        //_tetheredEnemy = null;
    }

    #endregion

    #region Helper Classes

    public enum ThrowState
    {
        Attached,
        Throwing,
        Arrived,
        Stuck,
        GrabbingEnemy,
        Returning
    }

    private class ElectricZoneInstanceClass
    {
        public ElectricZoneInstanceClass(GameObject electricZoneInstance)
        {
            ElectricZoneGameobject = electricZoneInstance;
            AttackHitBox = electricZoneInstance.GetComponent<WeaponAttackHitBox>();
            Lr = electricZoneInstance.GetComponent<LineRenderer>();
        }

        public GameObject ElectricZoneGameobject;
        public WeaponAttackHitBox AttackHitBox;
        public LineRenderer Lr;
    }

    #endregion
}