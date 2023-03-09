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
    private bool _invunerableToCrash = false;
    private float _prevVelocity;

    #endregion

    #region Public Properties

    public LayerMask CrashLayers => _crashLayers;
    public Rigidbody Rb => _rb;

    #endregion

    #region Getters & Setters

    public float MinCrashSpeed { get { return _minCrashSpeed; } set { _minCrashSpeed = value; } }
    public float CrashDamageMultiplier { get { return _crashDamageMultiplier; } set { _crashDamageMultiplier = value; } }
    public bool InvunerableToCrash { get { return _invunerableToCrash; } set { _invunerableToCrash = value; } }

    #endregion

    #region Unity Loops

    public override void Awake()
    {
        base.Awake();

        OnUpdateHealth += HandleUpdateHealth;
        OnDamaged += HandleDamaged;
        _boosterHealth.OnFix += HandleFix;

        _boosterHealth.SetMaxHealth((int)MaxHealth);
    }

    public override void Start()
    {
        base.Start();

        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _prevVelocity = _rb.velocity.magnitude;
    }

    private void OnDestroy()
    {
        OnUpdateHealth -= HandleUpdateHealth;
        OnDamaged -= HandleDamaged;
        _boosterHealth.OnFix -= HandleFix;
    }

    public override void OnCollisionStay(Collision collision)
    {
        base.OnCollisionStay(collision);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryInflictCrashDamage(collision);
    }

    #endregion

    /// <summary>
    /// Waits 1 second when shield is colliding so ship doesn't take damage when using shield
    /// </summary>
    public void SetInvunerableToCrash(float invunerableTime = 1f)
    {
        StartCoroutine(InvunerableToCrashCoroutine(invunerableTime));
    }

    private IEnumerator InvunerableToCrashCoroutine(float invunerableTime)
    {
        _invunerableToCrash = true;

        yield return new WaitForSeconds(invunerableTime);

        _invunerableToCrash = false;
    }

    public void TryInflictCrashDamage(Collision collision)
    {
        if (_invunerableToCrash) { return; }
        if ((1 << collision.gameObject.layer & _crashLayers) == 0) { return; }
        if (collision.gameObject.GetComponent<Projectile>() != null) { return; }
        if (_rb == null) { return; }
        if (_prevVelocity < _minCrashSpeed) { return; }

        _currentDamage = (int)CalculateCrashDamage();
        DamageWithoutDamageData((int)_currentDamage, collision.collider);
        if (collision.gameObject.TryGetComponent(out AIHealth enemyHealth)) { enemyHealth.Damage((int)_currentDamage); }

        float currentSpeedPercentage = _prevVelocity / Ship.Instance.TopSpeed;
        float crashImpactPercentageRatio = 4 * currentSpeedPercentage;
        float impactAmplitude = 5f * crashImpactPercentageRatio;
        ShipCamera.Instance.ShakeCamera(impactAmplitude, 50f, 0.2f);


        Vector3 hitPos = collision.contacts[0].point;
        GameObject shipCrashParticles = Instantiate(Ship.Instance.ShipStatsSO.ShipCrashParticles.gameObject, hitPos, Quaternion.identity);
    }

    public override void DamageWithoutDamageData(int damage, Collider instigatorCollider = null)
    {
        base.DamageWithoutDamageData(damage, instigatorCollider);

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

        CheckFlickerRedLights();
    }

    private void HandleDamaged(DamageTypes damageType)
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
