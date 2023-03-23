using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RepairMachine : Upgradable
{
    #region Editor Fields

    [Header("Stats")]
    [SerializeField] private int _healers = 2;
    [SerializeField] private int _amountPerHeal = 50;

    [Header("UI")]
    [SerializeField] private TextMeshPro _healersText;

    [Header("FX")]
    [SerializeField] private ParticleSystem _healParticles;

    #endregion

    #region Private Variables

    private ShipHealth _shipHealth;

    private float _timeSinceLastUsed;
    private float _timeBetweenUses = 0.5f;

    #endregion

    #region Unity Loops

    public override void Start()
    {
        base.Start();

        _shipHealth = FindObjectOfType<ShipHealth>();

        UpdateHealersUI();
    }

    private void Update()
    {
        UpdateTime();

        if (CurrentPlayer == null) { return; }

        CheckShootInput();
    }

    private void UpdateTime()
    {
        _timeSinceLastUsed += Time.deltaTime;
    }

    #endregion

    private void CheckShootInput()
    {
        if(_timeSinceLastUsed < _timeBetweenUses) { return; }

        if (CurrentPlayer.IsUsing)
        {
            UseHealer();
        }
    }

    private void UseHealer()
    {
        if (_healers < 1) { return; }

        _timeSinceLastUsed = 0f;

        _healers--;

        _shipHealth.AddHealth(_amountPerHeal);

        UpdateHealersUI();

        PlayHealFX();
    }

    private void UpdateHealersUI()
    {
        _healersText.text = _healers.ToString();
    }

    private void PlayHealFX()
    {
        _healParticles.Play();
    }
}
