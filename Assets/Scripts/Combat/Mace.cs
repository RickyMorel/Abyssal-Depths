using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mace : MeleeWeaponWithRope
{
    #region Editor Fields

    [Header("Floats")]
    [SerializeField] private float _pushForce = 20f;

    #endregion

    #region Private Variables

    private bool _prevInputState;

    #endregion

    #region Unity Loops

    public override void OnEnable()
    {
        //do nothing
    }

    public override void Update()
    {
        base.Update();

        if (_weapon.CurrentPlayer == null && _throwState == (MeleeWeaponWithRope.ThrowState)ThrowState.Throwing) { ReturnWeaponHead(); return; }
        if (_weapon.CurrentPlayer != null) { _throwState = (MeleeWeaponWithRope.ThrowState)ThrowState.Throwing; }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        ThrowWeaponHead();
    }

    public override void OnDisable()
    {
        //do nothing
    }

    #endregion

    public override void HandleHitParticles(GameObject obj)
    {
        if(obj.tag == "MainShip") { return; }

        if (obj.TryGetComponent(out AIStateMachine aIState))
        {
            Vector3 pushDir = _rb.velocity;
            aIState.BounceOffShield(pushDir, _pushForce);
        }
        Instantiate(GameAssetsManager.Instance.MeleeFloorHitParticles, _weaponHeadRb.transform.position, Quaternion.identity);
        ShipCamera.Instance.ShakeCamera(5f, 50f, 0.2f);
    }

    #region Shoot Functions



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