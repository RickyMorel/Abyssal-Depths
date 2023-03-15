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
    [SerializeField] private List<DamageTypes> _resistanceType = new List<DamageTypes>();
    [SerializeField] private List<DamageTypes> _weaknessType = new List<DamageTypes>();

    [Header("Stats")]
    [SerializeField] private float _currentHealth;
    [SerializeField] private int _maxHealth;

    [Header("UI")]
    [SerializeField] private Image _healthBarImage;

    [Header("FX")]
    [SerializeField] private ParticleSystem _damageParticles;

    #endregion

    #region Private Variables

    [ColorUsageAttribute(false, true)] private Color _originalColor;

    private Coroutine _fireRoutine = null;
    private Coroutine _electricRoutine = null;

    private float _timeSinceLastLaserShot = 0;
    private float _timeToResetLaserLevel = 2;
    private float _laserLevel;
    private float _damageTimer = 0;
    private float _timer = 0;
    private bool _isBeingElectrocuted = false;
    private DamageData _damageData;

    private ParticleSystem _fireParticles;
    private ParticleSystem _electricParticles;
    private Renderer[] _renderers;

    #endregion

    #region Getters & Setters

    public float CurrentHealth { get { return _currentHealth; } set { _currentHealth = value; } }
    public DamageData DamageData { get { return _damageData; } set { _damageData = value; } }
    public bool IsBeingElectrocuted { get { return _isBeingElectrocuted; } set { _isBeingElectrocuted = value; } }
    public ParticleSystem ElectricParticles { get { return _electricParticles; } set { _electricParticles = value; } }

    #endregion

    #region Public Properties
    public float MaxHealth => _maxHealth;

    public event Action<int> OnUpdateHealth;
    public event Action<DamageTypes> OnDamaged;
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

    public virtual void OnCollisionStay(Collision collision)
    {
        TryDamageWithProjectile(collision.collider);
    }

    private void OnTriggerStay(Collider other)
    {
        TryDamageWithProjectile(other);
    }

    private void TryDamageWithProjectile(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Projectile projectile)) { return; }

        //We don't want the projectile to damage its user
        if (IsOwnDamage(other)) { return; }

        _damageData = new DamageData(projectile.DamageData);
        ChangeColorForDamageTypeParticles(DamageData.ChipLevel);

        //In some cases, we don't want the projectile to be destroyed on impact, like the constant laser for example
        if (projectile.DestroyOnHit)
        {
            Damage(_damageData.Damage[0]);

            //If the projectile has 2 different damagetypes it will also do the damage for the second damagetype, if the second is none though, it will do its impact damage
            //or if both damagetypes are same, we don't want to do anything else
            if (projectile.DamageTypes[0] != projectile.DamageTypes[1] && projectile.DamageTypes[1] != DamageTypes.None) { Damage(_damageData.Damage[1], false, false, null, 1); }
            else if (projectile.DamageTypes[0] != projectile.DamageTypes[1] && projectile.DamageTypes[1] == DamageTypes.None) { Damage(_damageData.ImpactDamage, true); }

            projectile.DestroySelf();
        }
        else
        {
            //Since the projectile won't get destroyed on hit, we want it to do damage after a certain amount of seconds passes, or else it will just the damage everyframe
            _damageTimer += Time.deltaTime;
            if (_damageTimer >= projectile.DealDamageAfterSeconds)
            {
                Damage(_damageData.Damage[0]);
                _damageTimer = 0;
            }
        }
    }

    public void ChangeColorForDamageTypeParticles(int chipLevel)
    {
        if (_fireParticles != null) { GameAssetsManager.Instance.ChipDataSO.ChangeParticleColor(_fireParticles, DamageTypes.Fire, chipLevel); }
        if (_electricParticles != null) { GameAssetsManager.Instance.ChipDataSO.ChangeParticleColor(_electricParticles, DamageTypes.Electric, chipLevel); }
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

    public virtual void Damage(int damage, bool isImpactDamage = false, bool isDamageChain = false, Collider instigatorCollider = null, int index = 0)
    {
        if (IsDead()) { return; }

        if (_damageData == null) { return; }

        float finalDamage = 0;

        if (_damageData.DamageTypes[0] == DamageTypes.None)
        {
            finalDamage = damage;
        }
        else if(isImpactDamage)
        {
            finalDamage = damage;
        }

        bool isWeak = false;
        bool isResistant = false;

        if (!isImpactDamage && _damageData.DamageTypes[index] != DamageTypes.None)
        {
            if (_resistanceType.Contains(_damageData.DamageTypes[index]))
            {
                isResistant = true;
            }

            if (_weaknessType.Contains(_damageData.DamageTypes[index]))
            {
                isWeak = true;
            }

            finalDamage = DamageTypesSelector(_damageData.DamageTypes[index], isResistant, isWeak, (int)finalDamage, isDamageChain, index, damage);
        }

        _currentHealth = Mathf.Clamp(_currentHealth - finalDamage, 0, _maxHealth);

        UpdateHealthUI();

        OnDamaged?.Invoke(_damageData.DamageTypes[index]);

        if(finalDamage != 0) { DamagePopup.Create(transform.position, (int)finalDamage, _damageData.DamageTypes[index], isWeak); }

        if (_damageParticles != null && _damageData.DamageTypes[index] != DamageTypes.None) { _damageParticles.Play(); }

        if (_currentHealth == 0) { Die(); }
    }

    public virtual void DamageWithoutDamageData(int damage, Collider instigatorCollider = null)
    {
        if (IsDead()) { return; }

        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, _maxHealth);

        UpdateHealthUI();

        OnDamaged?.Invoke(DamageTypes.None);

        if (damage != 0) { DamagePopup.Create(transform.position, damage, DamageTypes.None, false); }

        if (_damageParticles != null) { _damageParticles.Play(); }

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

    private int DamageTypesSelector(DamageTypes damageType, bool isResistant, bool isWeak, int finalDamage, bool isDamageChain, int index, int damage)
    {
        if (DamageTypes.Electric == damageType) { finalDamage = ElectricDamage(isDamageChain, isResistant, isWeak, index, damage); }

        if (DamageTypes.Fire == damageType){ FireDamage(isResistant, isWeak, index, damage); }

        if (DamageTypes.Laser == damageType) { finalDamage = LaserDamage(isResistant, isWeak, damage, index); }

        if (DamageTypes.Base == damageType) { finalDamage = BaseDamage(isResistant, isWeak, index, damage); }

        return finalDamage;
    }

    private int LaserDamage(bool isResistant, bool isWeak, int damage, int index)
    {
        int laserDamage = CalculateDamageForTypes(isResistant, isWeak, index, damage);

        if (laserDamage == 0) { return damage; }

        laserDamage = laserDamage * (int)_laserLevel;

        _laserLevel = Mathf.Clamp(_laserLevel + 0.3f, 0f, 5f);

        _timeSinceLastLaserShot = 0;

        damage = damage + laserDamage;

        return damage;
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

    private void FireDamage(bool isResistant, bool isWeak, int index, int damage)
    {
        int fireDamage = CalculateDamageForTypes(isResistant, isWeak, index, damage);
        
        if (fireDamage == 0) { return; }

        if (DoesShowDamageParticles()) { _fireParticles.Play(); }

        if (_fireRoutine != null) { StopCoroutine(_fireRoutine); }

        _timer = 0;
        _fireRoutine = StartCoroutine(Afterburn(fireDamage, index));
    }

    private int ElectricDamage(bool isDamageChain, bool isResistant, bool isWeak, int index, int damage)
    {
        int electricDamage = CalculateDamageForTypes(isResistant, isWeak, index, damage);

        if (_isBeingElectrocuted && isDamageChain) { return 0; }

        _isBeingElectrocuted = true;

        if (TryGetComponent(out BaseStateMachine baseStateMachine)) { baseStateMachine.CanMove = false; }

        //Electrocute all nearby enemies
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _damageData.AdditionalValue[index], LayerMask.GetMask("NPC"));
        foreach (var hitCollider in hitColliders)
        {
            if (!hitCollider.TryGetComponent(out Damageable damageable)) { continue; }

            if (damageable is not AIHealth) { continue; }

            //Resets IsBeingElectrocuted to apply electric damage even though the monster is still being electrocuted
            if (_isBeingElectrocuted && !isDamageChain) { damageable.IsBeingElectrocuted = false; }
            damageable.DamageData = _damageData;
            damageable.ChangeColorForDamageTypeParticles(damageable.DamageData.ChipLevel);
            damageable.Damage(electricDamage, false, true, null, index);
        }

        if (_electricRoutine != null) { StopCoroutine(_electricRoutine); }

        _electricRoutine = StartCoroutine(ElectricParalysis(baseStateMachine, index));

        return electricDamage;
    }

    private int BaseDamage(bool isResistant, bool isWeak, int index, int damage)
    {
        int baseDamage = CalculateDamageForTypes(isResistant, isWeak, index, damage);
        return baseDamage;
    }

    private IEnumerator Afterburn(int damage, int index)
    {
        while (_timer < _damageData.SecondaryValue[index] && !IsDead())
        {
            yield return new WaitForSeconds(_damageData.AdditionalValue[index]);
            Damage(damage, true, false, null, index);
        }
        _fireParticles.Stop();
    }

    private IEnumerator ElectricParalysis(BaseStateMachine baseStateMachine, int index)
    {
        GAgent gAgent = GetComponent<GAgent>();
        if (DoesShowDamageParticles()) { _electricParticles.Play(); }
        yield return new WaitForSeconds(_damageData.SecondaryValue[index]);
        if (!IsDead() && baseStateMachine != null) { baseStateMachine.CanMove = true; }
        _isBeingElectrocuted = false;
        _electricParticles.Stop();
    }

    private int CalculateDamageForTypes(bool isResistant, bool isWeak, int index, int damage)
    {
        float damageAux = damage;

        if (isResistant && !isWeak) { damageAux = damageAux / _damageData.Resistance[index]; }

        if (isWeak && !isResistant) { damageAux = damageAux * _damageData.Weakness[index]; }

        return (int)damageAux;
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