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
    private Transform _ownersTransform;

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

        Invoke(nameof(DestroySelf), 4f);

        if (_weapon == null) { GameAssetsManager.Instance.EnemyDamageDataSO.CreateDamageForEnemies(_damageTypes, _aiCombatID, ref _damageData); }
        else 
        { 
            GameAssetsManager.Instance.ChipDataSO.CreateDamageDataFromChip(_damageTypes, _weapon, ref _damageData);

            if (_particles.Length < 1) { return; }

            GameAssetsManager.Instance.ChipDataSO.ChangeParticleColor(_particles[0], _damageTypes[0], _weapon.ChipLevel); 
        }
    }

    public void Launch(Vector3 direction, Vector3 lookDir = default(Vector3))
    {
        //Forces projectile to always be in Z = 0f
        direction.z = 0f;
        lookDir.z = 0f;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

        _rb.velocity = Vector3.zero;
        _rb.AddForce(direction.normalized * _speed, ForceMode.Impulse);

        if (lookDir == default(Vector3)) { return; }

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