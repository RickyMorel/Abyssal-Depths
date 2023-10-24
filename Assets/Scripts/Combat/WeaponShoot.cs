using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class WeaponShoot : MonoBehaviour
{
    #region Editor Fields

    [Header("Transforms")]
    [SerializeField] protected List<Transform> _shootTransforms = new List<Transform>();
    [SerializeField] protected Transform _turretHead;

    [Header("Stats")]
    [SerializeField] protected float _recoilForce = 2.5f;
    [SerializeField] protected float _timeBetweenShots = 0.2f;
    [SerializeField] protected WeaponSO _weaponSO;
    
    [Header("Sounds")]
    [SerializeField] private EventReference _shootingSfx;

    [Header("FX")]
    [SerializeField] private GameObject _projectileShellPrefab;
    [SerializeField] private float _recoilVisual = 1.2f; 

    #endregion

    #region Private Variable

    protected Weapon _weapon;
    protected float _timeSinceLastShot;

    private Transform _weaponHead;
    private ParticleSystem _shootBubbleParticles;
    private Vector3 _originalWeaponHeadPosition;

    #endregion

    #region Public Properties

    public Weapon Weapon => _weapon;

    #endregion

    #region Unity Loops

    public virtual void Start()
    {
        _turretHead = transform.GetChild(0);
        _weaponHead = _turretHead;

        FindShootTransforms();

        if (_weaponHead != null) { _originalWeaponHeadPosition = _weaponHead.transform.localPosition; }
        if (_shootTransforms.Count > 0) 
        {
            _shootBubbleParticles = WeaponFX.InstantiateBubbleParticles(_shootTransforms[0]);
        } 
    }

    public virtual void Update()
    {
        UpdateTime();
    }

    #endregion

    private void FindShootTransforms()
    {
        Transform[] childTransforms = GetComponentsInChildren<Transform>();

        foreach (Transform child in childTransforms)
        {
            if (child.name.Contains("ShootTransform"))
            {
                _shootTransforms.Add(child);
            }
        }
    }

    public void SetWeaponInteractable(Weapon weaponInteractable)
    {
        _weapon = weaponInteractable;
    }

    public virtual void CheckShootInput()
    {
        if(_weapon.CurrentPlayer == null) { return; }

        if (_weapon.CurrentPlayer.IsUsing)
        {
            Shoot();
        }
    }

    public virtual void Shoot()
    {
        if (_timeBetweenShots > _timeSinceLastShot) { return; }

        _timeSinceLastShot = 0f;
        InstantiateProjectile(_shootTransforms[0]);

        Ship.Instance.AddForceToShip(-_turretHead.transform.forward * _recoilForce, ForceMode.Impulse);

        PlayShootFX();
    }

    public void ProjectileShootFromOtherBarrels(int shootNumber)
    {
        InstantiateProjectile(_shootTransforms[shootNumber]);
    }

    private void InstantiateProjectile(Transform transform)
    {
        GameObject projectileInstance = Instantiate(_weaponSO.ProjectilePrefab, transform.position, transform.rotation);
        projectileInstance.GetComponent<Projectile>().WeaponReference = _weapon;
    }

    public void UpdateTime()
    {
        _timeSinceLastShot += Time.deltaTime;
    }

    public void PlayShootFX()
    {
		if (_shootTransforms.Count < 1) { return; }

        WeaponFX.PlayShootFX(this, _shootTransforms[0], _turretHead, _timeBetweenShots, _originalWeaponHeadPosition, _projectileShellPrefab, _shootingSfx, _shootBubbleParticles);

        //if (_shootBubbleParticles != null) { _shootBubbleParticles.Play(); }

        //if (_weaponHead != null) { StartCoroutine(PlayWeaponRecoilFX()); }

        //StartCoroutine(SpawnProjectileShell());
    }
}