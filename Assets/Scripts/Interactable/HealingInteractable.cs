using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingInteractable : Interactable
{
    #region Editor Fields

    [Range(0f, 100f)]
    [SerializeField] private float _healPercentageToAdd = 20f;
    [SerializeField] private ParticleSystem _healParticles;

    #endregion

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

        if(!CurrentPlayer.TryGetComponent<Damageable>(out Damageable damageable)) { return; }

        float healthToAdd = damageable.MaxHealth * (_healPercentageToAdd / 100f);

        damageable.AddHealth((int)healthToAdd);

        _healParticles.Play();
    }
}
