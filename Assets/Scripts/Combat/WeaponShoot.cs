using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShoot : MonoBehaviour
{
    #region Editor Fields

    [Header("Stats")]
    [SerializeField] protected float _recoilForce = 2.5f;
    [SerializeField] protected float _timeBetweenShots = 0.2f;

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
        _weaponHead = Weapon.TurretHead;
        if(_weaponHead != null) { _originalWeaponHeadPosition = _weaponHead.transform.localPosition; }
        _shootBubbleParticles = Instantiate(GameAssetsManager.Instance.ShootBubbleParticles, _weapon.ShootTransforms[0]).GetComponent<ParticleSystem>();
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
        if (_weapon.CurrentPlayer.IsUsing)
        {
            Shoot();
        }
    }

    public virtual void Shoot()
    {
        if (_timeBetweenShots > _timeSinceLastShot) { return; }

        _timeSinceLastShot = 0f;
        InstantiateProjectile(_weapon.ShootTransforms[0]);

        Ship.Instance.AddForceToShip(-_weapon.TurretHead.transform.forward * _recoilForce, ForceMode.Impulse);

        PlayShootFX();
    }

    public void ProjectileShootFromOtherBarrels(int shootNumber)
    {
        InstantiateProjectile(_weapon.ShootTransforms[shootNumber]);
    }

    private void InstantiateProjectile(Transform transform)
    {
        GameObject projectileInstance = Instantiate(_weapon.ProjectilePrefab, transform.position, _weapon.TurretHead.rotation);
        projectileInstance.GetComponent<Projectile>().WeaponReference = _weapon;
    }

    public void UpdateTime()
    {
        _timeSinceLastShot += Time.deltaTime;
    }

    private IEnumerator PlayWeaponRecoilFX()
    {
        Vector3 lookDir = (Weapon.ShootTransforms[0].position - transform.position).normalized;
        Vector3 desiredRecoilPosition = _weaponHead.transform.position +  -(lookDir) * (_recoilVisual);

        float elapsedTime = 0;
        float waitTime = _timeBetweenShots / 2f;


        //go backwards
        while (elapsedTime < waitTime)
        {
            Vector3 desiredPos = Vector3.Lerp(_weaponHead.transform.position, desiredRecoilPosition, (elapsedTime / waitTime));

            _weaponHead.transform.position = desiredPos;

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

        GameObject newShell = Instantiate(_projectileShellPrefab, Weapon.TurretHead.position, Quaternion.identity);

        Rigidbody rb = newShell.GetComponent<Rigidbody>();

        rb.AddForce(Weapon.TurretHead.up * 20f, ForceMode.Impulse);

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