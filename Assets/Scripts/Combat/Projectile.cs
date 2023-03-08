using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private float _speed;
    [SerializeField] protected DamageTypes[] _damageTypes;
    [SerializeField] protected ParticleSystem[] _particles;
    [SerializeField] protected bool _shouldUnparentParticle = false;

    #endregion

    #region Private Varaibles

    private Rigidbody _rb;
    protected bool _destroyOnHit = true;
    protected Weapon _weapon;
    protected int _aiCombatID;
    protected ChipDataSO _chipDataSO;
    protected EnemyDamageDataSO _enemyDamageDataSO;
    protected DamageData _damageData;
    protected float _dealDamageAfterSeconds;

    #endregion

    #region Public Properties

    public DamageTypes[] DamageTypes => _damageTypes;

    public bool DestroyOnHit => _destroyOnHit;
    public DamageData DamageData => _damageData;
    public Weapon Weapon => _weapon;
    public float DealDamageAfterSeconds => _dealDamageAfterSeconds;

    #endregion

    #region Getters and Setters

    public ParticleSystem[] ProjectileParticles { get { return _particles; } set { _particles = value; } }
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

        if (_weapon == null) { GameAssetsManager.Instance.EnemyDamageDataSO.CreateDamageForEnemies(_damageTypes, _aiCombatID, ref _damageData); }
        else 
        { 
            GameAssetsManager.Instance.ChipDataSO.CreateDamageDataFromChip(_damageTypes, _weapon, ref _damageData);

            if (_particles.Length < 1) { return; }

            GameAssetsManager.Instance.ChipDataSO.ChangeParticleColor(_particles[0], _damageTypes[0], _weapon.ChipLevel); 
        }
    }

    public void Initialize(string ownerTag)
    {
        gameObject.tag = ownerTag;
    }

    public void DestroySelf()
    {
        if (_shouldUnparentParticle) 
        {
            foreach (ParticleSystem particle in _particles)
                particle.transform.SetParent(null);
        }
        Destroy(gameObject);
    }

    #endregion
}