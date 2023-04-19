using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegalodonRagdollInteractable : Interactable
{
    private void Start()
    {
        Humble.OnInteract += HandleInteract;
    }

    private void OnDestroy()
    {
        Humble.OnInteract -= HandleInteract;
    }

    private void HandleInteract()
    {
        if(CurrentPlayer == null) { return; }

        if(!CurrentPlayer.TryGetComponent<MegalodonRagdollAttack>(out MegalodonRagdollAttack ragdollAttack)) { return; }

        ragdollAttack.BiteShip();
    }
}
