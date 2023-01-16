using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Stats/Ship", order = 1)]
public class ShipStatsSO : ScriptableObject
{
    [Header("Locomotion")]
    public float TopSpeed = 60f;

    [Header("Combat")]
    public int MaxHealth = 500;
    public float TimeTillDeath = 10f;

    [Header("Crashing")]
    public float MinCrashSpeed = 10f;
    public float CrashDamageMultiplier = 10f;

    [Header("Particle FX")]
    public ParticleSystem ShipCrashParticles;
    public ParticleSystem ShipEnemyDamageParticles;
    public ParticleSystem InteractableFriedParticles;
}
