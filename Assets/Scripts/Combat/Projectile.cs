using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private float _speed;
    [SerializeField] protected float _dealDamageAfterSeconds = 0;
    [SerializeField] protected DamageType _damageType;

    #endregion

    #region Private Varaibles

    private Rigidbody _rb;
    protected ParticleSystem _particles;
    protected bool _destroyOnHit = true;
    protected Weapon _weapon;
    protected int _damage;
    protected ChipDataSO.BasicChip _chipClass;
    protected ChipDataSO _chipDataSO;
    protected float _weakness;
    protected float _resistance;
    protected float _secondaryValue;
    protected float _impactDamage;
    protected float _additionalValue;

    #endregion

    #region Public Properties

    public int Damage => _damage;

    public DamageType DamageType => _damageType;

    public bool DestroyOnHit => _destroyOnHit;

    public float DealDamageAfterSeconds => _dealDamageAfterSeconds;

    public float Weakness => _weakness;
    public float Resistance => _resistance;
    public float SecondaryValue => _secondaryValue;
    public float ImpactDamage => _impactDamage;
    public float AdditionalValue => _additionalValue;

    #endregion

    #region Getters and Setters

    public ParticleSystem ProjectileParticles { get { return _particles; } set { _particles = value; } }
    public Weapon WeaponReference { get { return _weapon; } set { _weapon = value; } }

    #endregion

    #region Unity Loops

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _chipDataSO = GameAssetsManager.Instance.ChipDataSO;
    }

    public virtual void Start()
    {
        _rb.AddForce(transform.forward * _speed, ForceMode.Impulse);

        if (GetComponentInChildren<ParticleSystem>() == null) { return; }

        _particles = GetComponentInChildren<ParticleSystem>();
        Invoke(nameof(DestroySelf), 4f);

        _chipClass = _chipDataSO.GetChipType(_damageType);
        _chipDataSO.GetWeaknessAndResistance(_chipClass, out _weakness, out _resistance);
        _damage = _chipDataSO.GetDamageFromChip(_chipClass, _weapon.ChipLevel);
        _secondaryValue = _chipDataSO.GetSecondaryValueFromChip(_chipClass, _weapon.ChipLevel);

        if (_damageType == DamageType.Electric || _damageType == DamageType.Fire) { _impactDamage = _chipDataSO.GetImpactDamageFromChip(_chipClass); }
        if(_damageType == DamageType.Fire || _damageType == DamageType.Laser) { _additionalValue = _chipDataSO.GetAdditionalValueFromChip(_chipClass); }
    }

    public void Initialize(string ownerTag)
    {
        gameObject.tag = ownerTag;
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    #endregion
}