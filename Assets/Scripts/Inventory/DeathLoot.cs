using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathLoot : Lootable
{
    private void Start()
    {
        DestroyPrevLoot();
        Debug.Log("Spawned Death Loot");
    }

    private void OnDestroy()
    {
        Debug.Log("Destroyed Death Loot");
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
