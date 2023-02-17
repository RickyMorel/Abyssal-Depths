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
    [SerializeField] private int _damage = 20;
    [SerializeField] private DamageType _damageType;

    #endregion

    #region Private Varaibles

    private Rigidbody _rb;
    protected ParticleSystem _particles;
    protected bool _destroyOnHit = true;
    protected Weapon _weapon;

    #endregion

    #region Public Properties

    public int Damage => _damage;

    public DamageType DamageType => _damageType;

    public bool DestroyOnHit => _destroyOnHit;

    public float DealDamageAfterSeconds => _dealDamageAfterSeconds;


    #endregion

    #region Getters and Setters

    public ParticleSystem ProjectileParticles { get { return _particles; } set { _particles = value; } }
    public Weapon WeaponReference { get { return _weapon; } set { _weapon = value; } }

    #endregion

    #region Unity Loops

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public virtual void Start()
    {
        _rb.AddForce(transform.forward * _speed, ForceMode.Impulse);

        if (GetComponentInChildren<ParticleSystem>() == null) { return; }

        _particles = GetComponentInChildren<ParticleSystem>();

        Invoke(nameof(DestroySelf), 4f);
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