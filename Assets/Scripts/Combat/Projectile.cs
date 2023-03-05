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
    private Transform _ownersTransform;

    #endregion

    #region Public Properties

    public int Damage => _damage;

    public DamageType DamageType => _damageType;

    public bool DestroyOnHit => _destroyOnHit;

    public float DealDamageAfterSeconds => _dealDamageAfterSeconds;


    #endregion

    #region Getters and Setters

    public ParticleSystem ProjectileParticles { get { return _particles; } set { _particles = value; } }
    public Transform OwnersTransform { get { return _ownersTransform; } set { _ownersTransform = value; } }

    #endregion

    #region Unity Loops

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public virtual void Start()
    {
        Launch(transform.forward);

        if (GetComponentInChildren<ParticleSystem>() == null) { return; }

        _particles = GetComponentInChildren<ParticleSystem>();

        Invoke(nameof(DestroySelf), 4f);
    }

    public void Launch(Vector3 direction, Vector3 lookDir = default(Vector3))
    {
        _rb.velocity = Vector3.zero;
        _rb.AddForce(direction.normalized * _speed, ForceMode.Impulse);

        if (lookDir == default(Vector3)) { return; }

        Debug.DrawRay(transform.position, lookDir.normalized * _speed, Color.red, 4);
        transform.LookAt(lookDir.normalized * _speed);
    }

    public void Initialize(string ownerTag, Transform ownerTransform = null)
    {
        gameObject.tag = ownerTag;

        if(ownerTransform == null) { return; }

        _ownersTransform = ownerTransform;
    }

    public void RefelctFromShield(string newOwnerTag)
    {
        Vector3 newDir = _ownersTransform.position - transform.position;
        Launch(newDir, newDir);
        Initialize(newOwnerTag);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    #endregion
}