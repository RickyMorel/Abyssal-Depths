using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWhip : MeleeWeapon
{
    #region Editor Fields

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

        Instantiate(GameAssetsManager.Instance.FireWhipCollisionParticles, _attackHitBox.transform.position, Quaternion.identity);
        ShipCamera.Instance.ShakeCamera(2f, 50f, 0.2f);
    }
}