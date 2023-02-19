using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class Damageable : MonoBehaviour
{
    #region Editor Fields

    [Header("Type Resistances")]
    [SerializeField] private List<DamageType> _resistanceType = new List<DamageType>();
    [SerializeField] private List<DamageType> _weaknessType = new List<DamageType>();

    [Header("Stats")]
    [SerializeField] private int _maxHealth;

    [Header("UI")]
    [SerializeField] private Image _healthBarImage;

    [Header("FX")]
    [SerializeField] private ParticleSystem _damageParticles;

    #endregion

    #region Private Variables

    private float _currentHealth;

    [ColorUsageAttribute(false, true)] private Color _originalColor;

    private Coroutine _fireRoutine = null;
    private Coroutine _electricRoutine = null;

    private float _timeSinceLastLaserShot = 0;
    private float _timeToResetLaserLevel = 2;
    private float _laserLevel;
    private float _damageTimer = 0;
    private float _timer = 0;
    private bool _isBeingElectrocuted = false;

    private ParticleSystem _fireParticles;
    private ParticleSystem _electricParticles;
    private Renderer[] _renderers;

    private Projectile _projectile;

    #endregion

    #region Getters & Setters

    public float CurrentHealth { get { return _currentHealth; } set { _currentHealth = value; } }

    #endregion

    #region Public Properties
    public float MaxHealth => _maxHealth;

    public event Action<int> OnUpdateHealth;
    public event Action<DamageType> OnDamaged;
    public event Action OnDie;

    #endregion

    #region Unity Loops

    public virtual void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public virtual void Start()
    {
        FindMeshes();

        UpdateHealthUI();

        InstantiateDamageTypeParticles();
    }

    private void Update()
    {
        _timeSinceLastLaserShot += Time.deltaTime;
        _timer += Time.deltaTime;

        ColorChangeForLaser();
    }

    public virtual void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Projectile projectile)) { return; }

        if (IsOwnDamage(other)) { return; }

        if (projectile.DestroyOnHit)
        {
            _projectile = projectile;
            //LLamar 2 veces damage, a no ser que sean iguales
            Damage(projectile.Damage, projectile.DamageType, false);

            if (projectile.ProjectileParticles != null) { projectile.ProjectileParticles.transform.SetParent(null); }

            Destroy(projectile.gameObject);
        }
        else
        {
            _projectile = projectile;

            _damageTimer += Time.deltaTime;
            if (_damageTimer >= projectile.DealDamageAfterSeconds)
            {
                Damage(projectile.Damage, projectile.DamageType, false);
                _damageTimer = 0;
            }
        }
    }

    #endregion

    private void FindMeshes()
    {
        _renderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (Renderer renderer in _renderers)
        {
            renderer.material.EnableKeyword("_EMISSION");
            _originalColor = renderer.material.GetColor("_EmissionColor");
        }
    }

    public bool IsOwnDamage(Collider other)
    {
        //Turrets can't harm their own ship
        if (other.gameObject.tag == "Untagged" && gameObject.tag == "MainShip") { return true; }

        if (other.gameObject.tag == gameObject.tag) { return true; }

        return false;
    }

    private bool DoesShowDamageParticles()
    {
        if(this is InteractableHealth) { return false; }
        if(this is ShipHealth) { return false; }

        return true;
    }

    public void AddHealth(int amountAdded)
    {
        _currentHealth = Mathf.Clamp(_currentHealth + amountAdded, 0, _maxHealth);

        UpdateHealthUI();

        OnUpdateHealth?.Invoke(amountAdded);
    }

    public void SetHealth(int newHealth)
    {
        _currentHealth = Mathf.Clamp(newHealth, 0f, MaxHealth);

        OnUpdateHealth?.Invoke(newHealth);

        UpdateHealthUI();

        if (IsDead())
            Die();
    }

    public virtual void Damage(int damage, DamageType damageType = DamageType.None, bool damageOnHit = false, bool isDamageChain = false, Collider instigatorCollider = null)
    {
        if (IsDead()) { return; }

        float finalDamage = 0;

        if (damageType == DamageType.None || damageOnHit)
        {
            finalDamage = damage;
        }

        bool isWeak = false;
        bool isResistant = false;

        if (_resistanceType.Contains(damageType))
        { 
            isResistant = true;
        }

        if (_weaknessType.Contains(damageType))
        { 
            isWeak = true;
        }

        finalDamage = DamageTypesSelector(damageType, isResistant, isWeak, (int)finalDamage, isDamageChain);

        _currentHealth = Mathf.Clamp(_currentHealth - finalDamage, 0, _maxHealth);

        UpdateHealthUI();

        OnDamaged?.Invoke(damageType);

        if(finalDamage != 0) { DamagePopup.Create(transform.position, (int)finalDamage, isWeak); }

        if (_damageParticles != null && damageType != DamageType.None) { _damageParticles.Play(); }

        if (_currentHealth == 0) { Die(); }
    }

    public virtual void Die()
    {
        OnDie?.Invoke();
    }

    public bool IsDead()
    {
        return CurrentHealth <= 0f;
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        _maxHealth = newMaxHealth;
    }

    #region Damage Type Functions

    private void InstantiateDamageTypeParticles()
    {
        GameObject fireParticlesInstance = Instantiate(GameAssetsManager.Instance.FireParticles, transform);
        GameObject electricParticleInstance = Instantiate(GameAssetsManager.Instance.ElectricParticles, transform);

        _fireParticles = fireParticlesInstance.GetComponent<ParticleSystem>();
        _electricParticles = electricParticleInstance.GetComponent<ParticleSystem>();

        _fireParticles.Stop();
        _electricParticles.Stop();
    }

    private int DamageTypesSelector(DamageType damageType, bool isResistant, bool isWeak, int finalDamage, bool isDamageChain)
    {
        if (DamageType.Electric == damageType) { ElectricDamage(isDamageChain, isResistant, isWeak); }

        if (DamageType.Fire == damageType){ FireDamage(isResistant, isWeak); }

        if (DamageType.Laser == damageType) { finalDamage = LaserDamage(isResistant, isWeak, finalDamage); }

        if (DamageType.Base == damageType) { finalDamage = BaseDamage(isResistant, isWeak); }

        return finalDamage;
    }

    private int LaserDamage(bool isResistant, bool isWeak, int finalDamage)
    {
        int laserDamage = CalculateDamageForTypes(isResistant, isWeak);

        if (laserDamage == 0) { return finalDamage; }

        laserDamage = laserDamage * (int)_laserLevel;

        _laserLevel = Mathf.Clamp(_laserLevel + 0.3f, 0f, 5f);

        _timeSinceLastLaserShot = 0;

        finalDamage = finalDamage + laserDamage;

        return finalDamage;
    }

    private void ColorChangeForLaser()
    {
        if (_renderers.Length < 1) { return; }
        
        if (_timeSinceLastLaserShot > _timeToResetLaserLevel)
        {
            float laserReductionAmount = 2.5f * Time.deltaTime;
            _laserLevel = Mathf.Clamp(_laserLevel - laserReductionAmount, 0, 5);
        }

        foreach (Renderer renderer in _renderers)
        {
            renderer.material.SetColor("_EmissionColor", Color.Lerp(_originalColor, GameAssetsManager.Instance.LaserHeatColor, _laserLevel / 5f));
        }
    }

    private void FireDamage(bool isResistant, bool isWeak)
    {
        int fireDamage = CalculateDamageForTypes(isResistant, isWeak);

        if (fireDamage == 0) { return; }

        if (DoesShowDamageParticles()) { _fireParticles.Play(); }

        if (_fireRoutine != null) { StopCoroutine(_fireRoutine); }

        _timer = 0;
        _fireRoutine = StartCoroutine(Afterburn(fireDamage));
    }

    private void ElectricDamage(bool isDamageChain, bool isResistant, bool isWeak)
    {
        int electricDamage = CalculateDamageForTypes(isResistant, isWeak);

        if (electricDamage == 0) { return; }

        if (_isBeingElectrocuted && isDamageChain) { return; }

        _isBeingElectrocuted = true;

        if (TryGetComponent(out BaseStateMachine baseStateMachine)) { baseStateMachine.CanMove = false; }

        //Electrocute all nearby enemies
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10f);
        foreach (var hitCollider in hitColliders)
        {
            if (!hitCollider.TryGetComponent(out Damageable damageable)) { continue; }

            damageable.Damage(electricDamage, DamageType.Electric, true, true);
        }

        if (_electricRoutine != null) { StopCoroutine(_electricRoutine); }

        _electricRoutine = StartCoroutine(ElectricParalysis(baseStateMachine));
    }

    private int BaseDamage(bool isResistant, bool isWeak)
    {
        int baseDamage = CalculateDamageForTypes(isResistant, isWeak);
        return baseDamage;
    }

    private IEnumerator Afterburn(int damage)
    {
        while (_timer < _projectile.SecondaryValue)
        {
            yield return new WaitForSeconds(_projectile.AdditionalValue);
            Damage(damage);
        }
    }

    private IEnumerator ElectricParalysis(BaseStateMachine baseStateMachine)
    {
        if (DoesShowDamageParticles()) { _electricParticles.Play(); }
        yield return new WaitForSeconds(_projectile.SecondaryValue);
        if (!IsDead() && baseStateMachine != null) { baseStateMachine.CanMove = true; }
        _isBeingElectrocuted = false;
        _electricParticles.Stop();
    }

    private int CalculateDamageForTypes(bool isResistant, bool isWeak)
    {
        float damage = _projectile.Damage;

        if (isResistant && !isWeak) { damage = damage / _projectile.Resistance; }

        if (isWeak && !isResistant) { damage = damage * _projectile.Weakness; }

        return (int)damage;
    }

    #endregion

    #region UI

    public virtual void UpdateHealthUI()
    {
        if(_healthBarImage == null) { return; }

        _healthBarImage.fillAmount = CurrentHealth / MaxHealth;
    }

    #endregion
}