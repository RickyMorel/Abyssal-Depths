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
    [SerializeField] protected DamageTypes[] _damageTypes;
    [ColorUsageAttribute(false, true), SerializeField] private Color _laserHeatColor;

    #endregion

    #region Private Varaibles

    private Rigidbody _rb;
    protected ParticleSystem _particles;
    protected bool _destroyOnHit = true;
    protected Weapon _weapon;
    protected int _aiCombatID;
    protected int[] _damage = { 0, 0 };
    protected int _impactDamage;
    protected ChipDataSO.BasicChip[] _chipClass = {null, null};
    protected ChipDataSO _chipDataSO;
    protected EnemyDamageDataSO _enemyDamageDataSO;
    protected float[] _weakness = {0, 0};
    protected float[] _resistance = { 0, 0 };
    protected float[] _secondaryValue = { 0, 0 };
    protected float[] _additionalValue = { 0, 0 };
    private Renderer[] _renderers;
    private DamageData _damageData;

    #endregion

    #region Public Properties

    public int[] Damage => _damage;
    public int ImpactDamage => _impactDamage;

    public DamageTypes[] DamageTypes => _damageTypes;

    public bool DestroyOnHit => _destroyOnHit;

    public float DealDamageAfterSeconds => _dealDamageAfterSeconds;

    public float[] Weakness => _weakness;
    public float[] Resistance => _resistance;
    public float[] SecondaryValue => _secondaryValue;
    public float[] AdditionalValue => _additionalValue;
    public DamageData DamageData => _damageData;

    #endregion

    #region Getters and Setters

    public ParticleSystem ProjectileParticles { get { return _particles; } set { _particles = value; } }
    public Weapon WeaponReference { get { return _weapon; } set { _weapon = value; } }
    public int AICombatID { get { return _aiCombatID; } set { _aiCombatID = value; } }

    #endregion

    #region Unity Loops

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public virtual void Start()
    {
        _rb.AddForce(transform.forward * _speed, ForceMode.Impulse);
        Invoke(nameof(DestroySelf), 4f);

        if (_weapon == null) { CreateDamageForEnemies(); }
        else { CreateDamageDataFromChip(); }

        if (GetComponentInChildren<ParticleSystem>() == null) { return; }

        _particles = GetComponentInChildren<ParticleSystem>();
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

    private void CreateDamageDataFromChip()
    {
        _chipDataSO = GameAssetsManager.Instance.ChipDataSO;
        for (int i = 0; i < 2; i++)
        {
            _chipClass[i] = _chipDataSO.GetChipType(_damageTypes[i]);
            GameAssetsManager.Instance.DamageType.GetWeaknessAndResistance(_damageTypes[i], out _weakness[i], out _resistance[i]);
            _damage[i] = _chipDataSO.GetDamageFromChip(_chipClass[i], _weapon.ChipLevel, 0);
            _secondaryValue[i] = _chipDataSO.GetDamageFromChip(_chipClass[i], _weapon.ChipLevel, 1);
            
            if (_damageTypes[i] == global::DamageTypes.Fire || _damageTypes[i] == global::DamageTypes.Laser) { _additionalValue[i] = _chipDataSO.GetAdditionalValueFromChip(_chipClass[i]); }
        }
        _impactDamage = _chipDataSO.GetImpactDamageFromChip(_chipClass[0]);

        if (_damageTypes[0] == _damageTypes[1]) { _chipDataSO.GetBonusFromChip(_chipClass[0], ref _damage[0], ref _secondaryValue[0], ref _additionalValue[0]); }

        _damageData = new DamageData(_damageTypes, _damage, _impactDamage, _resistance, _weakness, _secondaryValue, _additionalValue);
    }

    private void CreateDamageForEnemies()
    {
        _enemyDamageDataSO = GameAssetsManager.Instance.EnemyDamageDataSO;
        _enemyDamageDataSO.EnemyDataDictionary.TryGetValue(_aiCombatID, out EnemyDamageValues enemyDamageValues);
        for (int i = 0; i < 2; i++)
        {
            _damage[i] = enemyDamageValues.Damage[i];
            _impactDamage = enemyDamageValues.ImpactDamage;
            _secondaryValue[i] = enemyDamageValues.SecondaryValue[i];
            _additionalValue[i] = enemyDamageValues.AdditionalValue[i];
            _damageTypes[i] = enemyDamageValues.DamageType[i];
            GameAssetsManager.Instance.DamageType.GetWeaknessAndResistance(_damageTypes[i], out _weakness[i], out _resistance[i]);
        }
        _damageData = new DamageData(_damageTypes, _damage, _impactDamage, _resistance, _weakness, _secondaryValue, _additionalValue);
    }
}