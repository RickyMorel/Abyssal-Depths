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
    [Header("Floats")]
    [SerializeField] private float _grappleSpeed;

    #endregion

    #region Private Variables

    private ThrowState _throwState = ThrowState.Attached;
    private Rigidbody _maceRb;
    private bool _prevInputState;
    private bool _prevInput2State;
    private float _timeSincePressInput;
    private LineRenderer _lr;
    private SpringJoint _tetherSpringInstance;

    #endregion

    public override void OnEnable()
    {
        _maceHead.transform.parent = null;
    }

    public override void OnDisable()
    {
        if (_parentTransform == null) { return; }

        _maceHead.transform.parent = _parentTransform;
    }

    public override void Start()
    {
        base.Start();

        _maceRb = _maceHead.GetComponent<Rigidbody>();
        _lr = GetComponent<LineRenderer>();
        _lr.positionCount = 2;
    }

    public override void HandleHitParticles(GameObject obj)
    {
        if(obj.tag == "MainShip") { return; }

        Instantiate(GameAssetsManager.Instance.MeleeFloorHitParticles, _maceHead.transform.position, Quaternion.identity);
        ShipCamera.Instance.ShakeCamera(5f, 50f, 0.2f);
    }

    #region Shoot Functions

    public override void CheckShootInput()
    {
        //Grapple ship towards harpoon
        if (_weapon.CurrentPlayer.IsUsing_2 && _throwState == ThrowState.Stuck)
        {
            Vector3 grappleDirection = _maceRb.transform.position - Ship.Instance.transform.position;
            Vector3 finalForce = grappleDirection.normalized * _grappleSpeed * Time.deltaTime;
            Ship.Instance.AddForceToShip(finalForce, ForceMode.Force);
            return;
        }

        //CheckShootBeacons();

        //Shoots harpoon
        if (_weapon.CurrentPlayer.IsUsing == _prevInputState) { return; }

        _prevInputState = _weapon.CurrentPlayer.IsUsing;

        if (_weapon.CurrentPlayer.IsUsing)
        {
            Shoot();
        }
    }

    private void CheckShootBeacons()
    {
        if (_weapon.CurrentPlayer.PlayerInput.DetectDoubleTap())
        {
            //DestroyBeacons();
            return;
        }

        if (_timeSincePressInput < 0.4f) { return; }

        if (_weapon.CurrentPlayer.IsUsing_2 == _prevInput2State) { return; }

        _prevInput2State = _weapon.CurrentPlayer.IsUsing_2;

        _timeSincePressInput = 0f;

        //Shoot electric wire beacons
        if (_weapon.CurrentPlayer.IsUsing_2 && _throwState != ThrowState.Stuck && _throwState != ThrowState.GrabbingEnemy)
        {
            //TrySpawnWireOrSpawnBeacon();

            return;
        }
    }

    private void DrawRope()
    {
        _lr.SetPosition(0, _maceRb.transform.position);
        _lr.SetPosition(1, _handleTransform.position);
    }

    private void CreateSpringObject(AIStateMachine enemy = null)
    {
        Destroy(_tetherSpringInstance);

        if (_throwState != ThrowState.Stuck && _throwState != ThrowState.GrabbingEnemy) { return; }

        _tetherSpringInstance = Ship.Instance.gameObject.AddComponent<SpringJoint>();

        _tetherSpringInstance.connectedBody = enemy ? enemy.Rb : _maceRb;
        _tetherSpringInstance.maxDistance = Vector3.Distance(Ship.Instance.transform.position, _maceRb.transform.position);
        _tetherSpringInstance.massScale = Ship.Instance.Rb.mass;
        _tetherSpringInstance.connectedMassScale = enemy ? enemy.Rb.mass : 1f;
        //_tetheredEnemy = enemy;
    }

    private void ReturnHarpoon()
    {
        _throwState = ThrowState.Returning;
        Destroy(_tetherSpringInstance);

        //if (_tetheredEnemy == null) { return; }

        //_tetheredEnemy.SetRagdollState(false);
        //_tetheredEnemy = null;
    }

    public override void Shoot()
    {
        if (_throwState != ThrowState.Attached)
        {
            if (_throwState == ThrowState.Arrived || _throwState == ThrowState.Stuck || _throwState == ThrowState.GrabbingEnemy) { }//ReturnHarpoon(); }

            return;
        }

        _throwState = ThrowState.Throwing;
        _maceRb.isKinematic = false;
        _weapon.ShouldRotate = false;

        _maceRb.transform.SetParent(null);
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