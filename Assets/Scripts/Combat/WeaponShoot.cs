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
    [SerializeField] private GameObject _muzzleFlash; 

    #endregion

    #region Private Variable

    protected Weapon _weapon;
    protected float _timeSinceLastShot;

    private Transform _weaponHead;
    private ParticleSystem _shootBubbleParticles;
    private float _originalWeaponHeadYPosition;

    #endregion

    #region Public Properties

    public Weapon Weapon => _weapon;

    #endregion

    #region Unity Loops

    public virtual void Start()
    {
        _weaponHead = transform.Find("WeaponHead");
        if(_weaponHead != null) { _originalWeaponHeadYPosition = _weaponHead.transform.localPosition.y; }
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

        StartCoroutine(PlayShootFX());

        if(_weaponHead != null) { StartCoroutine(PlayWeaponRecoilFX()); }
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
        Debug.Log("PlayWeaponRecoilFX");
        float desiredRecoilPosition = _weaponHead.transform.localPosition.y / 2;
        float elapsedTime = 0;
        float waitTime = _timeBetweenShots / 2f;


        //go backwards
        while (elapsedTime < waitTime)
        {
            Debug.Log("Lerping...");

            float desiredY = Mathf.Lerp(_weaponHead.transform.localPosition.y, desiredRecoilPosition, (elapsedTime / waitTime));

            _weaponHead.transform.localPosition = new Vector3(_weaponHead.transform.localPosition.x, desiredY, _weaponHead.transform.localPosition.z);

            elapsedTime += Time.deltaTime;

            yield return 0;
        }

        elapsedTime = 0;

        //go forwards
        while (elapsedTime < waitTime)
        {
            Debug.Log("Lerping...");

            float desiredY = Mathf.Lerp(_weaponHead.transform.localPosition.y, _originalWeaponHeadYPosition, (elapsedTime / waitTime));

            _weaponHead.transform.localPosition = new Vector3(_weaponHead.transform.localPosition.x, desiredY, _weaponHead.transform.localPosition.z);

            elapsedTime += Time.deltaTime;

            yield return 0;
        }
    }

    private IEnumerator PlayShootFX()
    {
        _shootBubbleParticles.Play();

        //if(_muzzleFlash != null) { _muzzleFlash.SetActive(true); }

        yield return new WaitForSeconds(0.1f);

        //if (_muzzleFlash != null) { _muzzleFlash.SetActive(false); }
    }
}