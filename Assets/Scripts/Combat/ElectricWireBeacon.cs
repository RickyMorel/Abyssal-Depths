using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricWireBeacon : Projectile
{
    #region Private Variables

    private bool _hasLaunched = false;

    #endregion


    public override void Awake()
    {
        base.Awake();

        _destroyOnHit = false;
    }

    //Do nothing
    public override void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (!_hasLaunched) 
        {
            if (Rb.velocity.magnitude > Speed / 2f) { _hasLaunched = true; } 

            return; 
        }

        if(Rb.velocity.magnitude < 3f) { Rb.isKinematic = true; }
    }
}
