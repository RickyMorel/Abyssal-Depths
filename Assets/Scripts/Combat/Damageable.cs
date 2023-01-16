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
    private bool _isBeingElectrocuted = false;

    private ParticleSystem _fireParticles;
    private ParticleSystem _electricParticles;
    private Renderer[] _renderers;

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

    public virtual void Start()
    {
        _currentHealth = _maxHealth;

        FindMeshes();

        UpdateHealthUI();

        InstantiateDamageTypeParticles();
    }

    private void Update()
    {
        _timeSinceLastLaserShot += Time.deltaTime;

        ColorChangeForLaser();
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent<Projectile>(out Projectile projectile)) { return; }

        if (IsOwnDamage(other)) { return; }

        Damage(projectile.Damage, projectile.DamageType, false);

        if (projectile.ProjectileParticles != null) { projectile.ProjectileParticles.transform.SetParent(null); }

        Destroy(projectile.gameObject);
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
        _currentHealth = newHealth;

        UpdateHealthUI();

        if (IsDead())
            Die();
    }

    public virtual void Damage(int damage, DamageType damageType = DamageType.None, bool isDamageChain = false, Collider instigatorCollider = null)
    {
        if (IsDead()) { return; }

        int finalDamage = damage;

        bool isWeak = false;
        bool isResistant = false;

        if (_resistanceType.Contains(damageType)) 
        { 
            finalDamage = finalDamage / 2;
            isResistant = true;
        }

        if (_weaknessType.Contains(damageType)) 
        { 
            finalDamage = finalDamage * 2;
            isWeak = true;
        }

        finalDamage = DamageTypesSelector(damageType, isResistant, isWeak, finalDamage, isDamageChain);

        _currentHealth = Mathf.Clamp(_currentHealth - finalDamage, 0, _maxHealth);

        UpdateHealthUI();

        OnDamaged?.Invoke(damageType);

        if (_damageParticles != null && damageType != DamageType.None) { _damageParticles.Play(); }

        if (_currentHealth == 0)
            Die();
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
        if (DamageType.Electric == damageType) { ElectricDamage(isDamageChain); }

        if (DamageType.Fire == damageType){ FireDamage(isResistant, isWeak); }

        if (DamageType.Laser == damageType) { finalDamage = LaserDamage(isResistant, isWeak, finalDamage); }

        return finalDamage;
    }

    private int LaserDamage(bool isResistant, bool isWeak, int finalDamage)
    {
        int laserDamage = 0;

        _laserLevel = Mathf.Clamp(_laserLevel + 0.3f, 0f, 5f);

        _timeSinceLastLaserShot = 0;

        if ((!isResistant && !isWeak) || (isResistant && isWeak)) { laserDamage = 8 * (int)_laserLevel; }

        if (isResistant && !isWeak) { laserDamage = 4 * (int)_laserLevel; }

        if (isWeak && !isResistant) { laserDamage = 12 * (int)_laserLevel; }

        if (laserDamage == 0) { return finalDamage; }

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
        if (DoesShowDamageParticles()) { _fireParticles.Play(); }

        int fireDamage = 0;

        if ((!isResistant && !isWeak) || (isResistant && isWeak)) { fireDamage = 8; }

        if (isResistant && !isWeak) { fireDamage = 4; }

        if (isWeak && !isResistant) { fireDamage = 12; }

        if (fireDamage == 0) { return; }

        if (_fireRoutine != null) { StopCoroutine(_fireRoutine); }

        _fireRoutine = StartCoroutine(Afterburn(fireDamage));
    }

    private void ElectricDamage(bool isDamageChain)
    {
        if (_isBeingElectrocuted && isDamageChain) { return; }

        _isBeingElectrocuted = true;

        if (TryGetComponent<BaseStateMachine>(out BaseStateMachine baseStateMachine)) { baseStateMachine.CanMove = false; }

        //Electrocute all nearby enemies
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10f);
        foreach (var hitCollider in hitColliders)
        {
            if (!hitCollider.TryGetComponent<Damageable>(out Damageable damageable)) { continue; }

            damageable.Damage(2, DamageType.Electric, true);
        }

        if (_electricRoutine != null) { StopCoroutine(_electricRoutine); }

        _electricRoutine = StartCoroutine(ElectricParalysis(baseStateMachine));
    }

    private IEnumerator Afterburn(int damage)
    {
        yield return new WaitForSeconds(1);
        Damage(damage, DamageType.None);
        yield return new WaitForSeconds(1);
        Damage(damage, DamageType.None);
        yield return new WaitForSeconds(1);
        Damage(damage, DamageType.None);
        yield return new WaitForSeconds(1);
        Damage(damage, DamageType.None);
        yield return new WaitForSeconds(1);
        Damage(damage, DamageType.None);
        _fireParticles.Stop();
    }

    private IEnumerator ElectricParalysis(BaseStateMachine baseStateMachine)
    {
        if (DoesShowDamageParticles()) { _electricParticles.Play(); }
        yield return new WaitForSeconds(2);
        if (!IsDead() && baseStateMachine != null) { baseStateMachine.CanMove = true; }
        _isBeingElectrocuted = false;
        _electricParticles.Stop();
    }

    #endregion

    #region UI

    public void UpdateHealthUI()
    {
        if(_healthBarImage == null) { return; }

        _healthBarImage.fillAmount = CurrentHealth / MaxHealth;
    }

    #endregion
}