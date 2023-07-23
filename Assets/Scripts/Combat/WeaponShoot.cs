using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class WeaponShoot : MonoBehaviour
{
    #region Editor Fields

    [Header("Transforms")]
    [SerializeField] protected Transform[] _shootTransforms;
    [SerializeField] protected Transform _turretHead;

    [Header("Stats")]
    [SerializeField] protected float _recoilForce = 2.5f;
    [SerializeField] protected float _timeBetweenShots = 0.2f;
    
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
        if(_weaponHead != null) { _originalWeaponHeadPosition = _weaponHead.transform.localPosition; }
        _shootBubbleParticles = Instantiate(GameAssetsManager.Instance.ShootBubbleParticles, _shootTransforms[0]).GetComponent<ParticleSystem>();
        _shootBubbleParticles.transform.localPosition = Vector3.zero;
        _shootBubbleParticles.transform.localRotation = Quaternion.identity;   
    }

    public virtual void Update()
    {
        UpdateTime();
    }

    #endregion

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
        GameAudioManager.Instance.PlaySound(_shootingSfx, transform.position);
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
        GameObject projectileInstance = Instantiate(_weapon.ProjectilePrefab, transform.position, transform.rotation);
        projectileInstance.GetComponent<Projectile>().WeaponReference = _weapon;
    }

    public void UpdateTime()
    {
        _timeSinceLastShot += Time.deltaTime;
    }

    private IEnumerator PlayWeaponRecoilFX()
    {
        Vector3 lookDir = (_shootTransforms[0].position - transform.position).normalized;
        Vector3 desiredRecoilPosition = _weaponHead.transform.localPosition +  -lookDir * _recoilVisual;

        float elapsedTime = 0;
        float waitTime = _timeBetweenShots / 2f;


        //go backwards
        while (elapsedTime < waitTime)
        {
            Vector3 desiredPos = Vector3.Lerp(_weaponHead.transform.localPosition, desiredRecoilPosition, (elapsedTime / waitTime));

            _weaponHead.transform.localPosition = desiredPos;

            elapsedTime += Time.deltaTime;

            yield return 0;
        }

        elapsedTime = 0;

        //go forwards
        while (elapsedTime < waitTime)
        {
            Vector3 desiredPos = Vector3.Lerp(_weaponHead.transform.localPosition, _originalWeaponHeadPosition, (elapsedTime / waitTime));

            _weaponHead.transform.localPosition = desiredPos;

            elapsedTime += Time.deltaTime;

            yield return 0;
        }
    }

    private IEnumerator SpawnProjectileShell()
    {
        if(_projectileShellPrefab == null) { yield break; }

        GameObject newShell = Instantiate(_projectileShellPrefab, _turretHead.position, Quaternion.identity);

        Rigidbody rb = newShell.GetComponent<Rigidbody>();

        rb.AddForce(_turretHead.up * 20f, ForceMode.Impulse);

        yield return new WaitForSeconds(30f);

        rb.isKinematic = true;
    }

    public void PlayShootFX()
    {
        _shootBubbleParticles.Play();

        if (_weaponHead != null) { StartCoroutine(PlayWeaponRecoilFX()); }

        StartCoroutine(SpawnProjectileShell());
    }
}