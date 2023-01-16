using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipHealth : Damageable
{
    #region Editor Fields

    [Header("Crash Parameters")]
    [SerializeField] private InteractableHealth _boosterHealth;
    [SerializeField] private LayerMask _crashLayers;

    [Header("FX")]
    [SerializeField] private GameObject _redLights;

    #endregion

    #region Private Varaibles

    private Rigidbody _rb;
    private float _currentDamage;
    private float _minCrashSpeed;
    private float _crashDamageMultiplier;

    #endregion

    #region Getters & Setters

    public float MinCrashSpeed { get { return _minCrashSpeed; } set { _minCrashSpeed = value; } }
    public float CrashDamageMultiplier { get { return _crashDamageMultiplier; } set { _crashDamageMultiplier = value; } }

    #endregion

    #region Unity Loops

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _boosterHealth.SetMaxHealth((int)MaxHealth);
    }

    public override void Start()
    {
        base.Start();

        OnUpdateHealth += HandleUpdateHealth;
        OnDamaged += HandleDamaged;
        _boosterHealth.OnFix += HandleFix;
    }

    private void OnDestroy()
    {
        OnUpdateHealth -= HandleUpdateHealth;
        OnDamaged -= HandleDamaged;
        _boosterHealth.OnFix -= HandleFix;
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if((1<<other.gameObject.layer & _crashLayers) == 0) { return; }
        if(other.gameObject.GetComponent<Projectile>() != null) { return; }
        if(_rb.velocity.magnitude < _minCrashSpeed) { return; }

        _currentDamage = (int)CalculateCrashDamage();
        Damage((int)_currentDamage, DamageType.Base);
        if (other.TryGetComponent<AIHealth>(out AIHealth enemyHealth)) { enemyHealth.Damage((int)_currentDamage); }

        float currentSpeedPercentage = _rb.velocity.magnitude / Ship.Instance.TopSpeed;
        float crashImpactPercentageRatio = 4 * currentSpeedPercentage;
        float impactAmplitude = 5f * crashImpactPercentageRatio;
        ShipCamera.Instance.ShakeCamera(impactAmplitude, 50f, 0.2f);


        Vector3 hitPos = other.ClosestPointOnBounds(transform.position);
        GameObject shipCrashParticles = Instantiate(Ship.Instance.ShipStatsSO.ShipCrashParticles.gameObject, hitPos, Quaternion.identity);
    }

    #endregion

    public override void Damage(int damage, DamageType damageType = DamageType.None, bool isDamageChain = false, Collider instigatorCollider = null)
    {
        base.Damage(damage, damageType, isDamageChain);

        ShipCamera.Instance.ShakeCamera(2f, 50f, 0.2f);

        if(instigatorCollider == null) { return; }

        Vector3 hitPos = instigatorCollider.ClosestPointOnBounds(transform.position);
        GameObject shipEnemyDamageParticles = Instantiate(Ship.Instance.ShipStatsSO.ShipEnemyDamageParticles.gameObject, hitPos, Quaternion.identity);
    }

    private void HandleFix()
    {
        AddHealth((int)MaxHealth);
    }

    private float CalculateCrashDamage()
    {
        float finalDamage = _rb.velocity.magnitude* _crashDamageMultiplier;

        return finalDamage;
    }

    private void HandleUpdateHealth(int healthAdded)
    {
        _boosterHealth.SetHealth((int)CurrentHealth);
    }

    private void HandleDamaged(DamageType damageType)
    {
        _boosterHealth.SetHealth((int)CurrentHealth);

        CheckFlickerRedLights();
    }

    private void CheckFlickerRedLights()
    {
        if (!IsDead()) { return; }

        StartCoroutine(FlickerRedLights());
    }

    private IEnumerator FlickerRedLights()
    {
        _redLights.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        _redLights.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        if (!IsDead()) { yield return null; }
        else { StartCoroutine(FlickerRedLights()); }
    }
}
