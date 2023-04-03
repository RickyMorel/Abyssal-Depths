using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWhip : MeleeWeapon
{
    public override void HandleHitParticles(GameObject obj)
    {
        if(obj.tag == "MainShip") { return; }

        Instantiate(GameAssetsManager.Instance.FireWhipCollisionParticles, _attackHitBox.transform.position, Quaternion.identity);
        ShipCamera.Instance.ShakeCamera(2f, 50f, 0.2f);
    }
}