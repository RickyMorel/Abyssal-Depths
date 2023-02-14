using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathLoot : Lootable
{
    [SerializeField] private Transform _beamParticles;

    private void Start()
    {
        DestroyPrevLoot();
    }

    private void FixedUpdate()
    {
        //Make particles always face up
        _beamParticles.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
    }

    private void DestroyPrevLoot()
    {
        DeathLoot[] prevLoot = FindObjectsOfType<DeathLoot>();

        foreach (DeathLoot loot in prevLoot)
        {
            if(loot == this) { continue; }

            Destroy(loot.gameObject);
        }
    }
}
