using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mace : MeleeWeapon
{
    #region Editor Fields

    [SerializeField] private GameObject _maceHead;

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

    public override void HandleHitParticles(GameObject obj)
    {
        if(obj.tag == "MainShip") { return; }

        Instantiate(GameAssetsManager.Instance.MeleeFloorHitParticles, _maceHead.transform.position, Quaternion.identity);
        ShipCamera.Instance.ShakeCamera(5f, 50f, 0.2f);
    }
}