using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWhip : MeleeWeapon
{
    #region Editor Fields

    [SerializeField] private float _pushForce = 20f;
    [SerializeField] private AttackHitBox[] _attackHitBoxes;

    #endregion

    public override void Awake()
    {
        base.Awake();

        foreach (AttackHitBox hitBox in _attackHitBoxes)
        {
            hitBox.OnHit += HandleHitParticles;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        foreach (AttackHitBox hitBox in _attackHitBoxes)
        {
            hitBox.OnHit -= HandleHitParticles;
        }
    }

    public override void HandleHitParticles(GameObject obj)
    {
        if(obj.tag == "MainShip") { return; }

        if(obj.TryGetComponent(out AIStateMachine aIState)) 
        {
            Vector3 pushDir = _rb.velocity;
            aIState.BounceOffShield(pushDir, _pushForce);
        }
        Instantiate(GameAssetsManager.Instance.FireWhipCollisionParticles, _attackHitBox.transform.position, Quaternion.identity);
        ShipCamera.Instance.NormalShake();
    }
}