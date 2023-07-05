using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    #region Editor Fields

    [Header("Damage Data")]
    [SerializeField] private float _speed;
    [SerializeField] protected DamageTypes[] _damageTypes;
    [SerializeField] protected bool _stunPlayersInShipOnHit = false;

    [Header("FX")]
    [SerializeField] protected ParticleSystem[] _particles;
    [SerializeField] private GameObject _muzzleFlash;
    [SerializeField] protected bool _shouldUnparentParticle = false;
    [SerializeField] protected bool _shakeCameraOnHit = false;

    #endregion

    #region Private Varaibles

    private Rigidbody _rb;
    protected bool _destroyOnHit = true;
    protected Weapon _weapon;
    protected int _aiCombatID;
    protected DamageData _damageData;
    protected float _dealDamageAfterSeconds;
    private Transform _ownersTransform;
    private GameObject _projectileMesh;

    #endregion

    #region Public Properties

    public DamageTypes[] DamageTypes => _damageTypes;

    public bool DestroyOnHit => _destroyOnHit;
    public DamageData DamageData => _damageData;
    public Rigidbody Rb => _rb;
    public float DealDamageAfterSeconds => _dealDamageAfterSeconds;
    public bool ShakeCameraOnHit => _shakeCameraOnHit;

    public float Speed => _speed;

    #endregion

    #region Getters and Setters

    public ParticleSystem[] ProjectileParticles { get { return _particles; } set { _particles = value; } }
    public Weapon WeaponReference { get { return _weapon; } set { _weapon = value; } }
    public int AICombatID { get { return _aiCombatID; } set { _aiCombatID = value; } }

    #endregion

    #region Unity Loops

    public virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public virtual void Start()
    {
        _projectileMesh = transform.Find("Mesh").gameObject;

        _damageData = DamageData.GetDamageData(_damageTypes, _weapon, _aiCombatID);

        ApplySlightRandomRotation();
        StartCoroutine(TryPlayMuzzleFlash());

        if (_destroyOnHit == true) 
        {
            Launch(transform.forward);
            Invoke(nameof(DestroySelf), 4f); 
        }

        if (_weapon == null) { return; }

        if (_particles.Length < 1) { return; }

        GameAssetsManager.Instance.ChipDataSO.ChangeParticleColor(_particles[0], _damageTypes[0], _weapon.ChipLevel); 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Damageable.IsOwnDamage(collision.collider, gameObject)) { return; }

        PlayImpactParticles(collision.contacts[0].point);

        if (_destroyOnHit) { Destroy(gameObject); }
    }

    private void ApplySlightRandomRotation()
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x + Random.Range(-15f, 15f), transform.eulerAngles.y, transform.eulerAngles.z);
    }

    private IEnumerator TryPlayMuzzleFlash()
    {
        if (_muzzleFlash == null) { yield break; }

        _projectileMesh.SetActive(false);
        _muzzleFlash.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        _muzzleFlash.SetActive(false);
        _projectileMesh.SetActive(true);
    }

    public void Launch(Vector3 direction, Vector3 lookDir = default(Vector3))
    {
        //Forces projectile to always be in Z = 0f
        direction.z = 0f;
        lookDir.z = 0f;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

        _rb.velocity = Vector3.zero;
        _rb.AddForce(direction.normalized * _rb.mass * _speed, ForceMode.Impulse);

        if (lookDir == default(Vector3)) { return; }

        transform.LookAt(lookDir.normalized * _speed);
    }

    public void Initialize(string ownerTag, Transform ownerTransform = null)
    {
        gameObject.tag = ownerTag;

        if(ownerTransform == null) { return; }

        _ownersTransform = ownerTransform;
    }

    public void ReflectFromShield(string newOwnerTag)
    {
        Vector3 newDir = _ownersTransform.position - transform.position;
        Launch(newDir, newDir);
        Initialize(newOwnerTag);
    }

    public void PlayImpactParticles(Vector3 impactPoint)
    {
        foreach (ParticleSystem particle in _particles)
        {
            if (_shouldUnparentParticle) { particle.transform.SetParent(null); }
            particle.transform.position = impactPoint;
            particle.Play();
        }
    }

    public void TryRagdollPlayers()
    {
        if (!_stunPlayersInShipOnHit) { return; }

        Ship.Instance.ShipHealth.RagdollPlayersInShip();
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    #endregion
}