using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLaser : Projectile
{
    public override void Start()
    {
        if (GetComponentInChildren<ParticleSystem>() == null) { return; }

        _particles = GetComponentInChildren<ParticleSystem>();
    }
}