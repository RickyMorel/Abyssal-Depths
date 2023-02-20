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
    [SerializeField] protected DamageType[] _damageTypes;
    [ColorUsageAttribute(false, true), SerializeField] private Color _laserHeatColor;

    #endregion

    #region Private Varaibles

    private Rigidbody _rb;
    protected ParticleSystem _particles;
    protected bool _destroyOnHit = true;
    protected Weapon _weapon;
    protected int[] _damage;
    protected ChipDataSO.BasicChip[] _chipClass;
    protected ChipDataSO _chipDataSO;
    protected float[] _weakness;
    protected float[] _resistance;
    protected float[] _secondaryValue;
    protected float[] _additionalValue;
    private Renderer[] _renderers;

    #endregion

    #region Public Properties

    public int[] Damage => _damage;

    public DamageType[] DamageTypes => _damageTypes;

    public bool DestroyOnHit => _destroyOnHit;

    public float DealDamageAfterSeconds => _dealDamageAfterSeconds;

    public float[] Weakness => _weakness;
    public float[] Resistance => _resistance;
    public float[] SecondaryValue => _secondaryValue;
    public float[] AdditionalValue => _additionalValue;

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
        for (int i = 0; i < 2; i++)
        {
            _chipClass[i] = _chipDataSO.GetChipType(_damageTypes[i]);
            _chipDataSO.GetWeaknessAndResistance(_chipClass[i], out _weakness[i], out _resistance[i]);
            _damage[0] = _chipDataSO.GetDamageFromChip(_chipClass[i], _weapon.ChipLevel, 0);
            _secondaryValue[i] = _chipDataSO.GetDamageFromChip(_chipClass[i], _weapon.ChipLevel, 1);

            if (_damageTypes[i] == DamageType.Fire || _damageTypes[i] == DamageType.Laser) { _additionalValue[i] = _chipDataSO.GetAdditionalValueFromChip(_chipClass[i]); }
        }

        if (_damageTypes[0] == _damageTypes[1]) { _chipDataSO.GetBonusFromChip(_chipClass[0], ref _damage[0], ref _secondaryValue[0], ref _additionalValue[0]); }
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