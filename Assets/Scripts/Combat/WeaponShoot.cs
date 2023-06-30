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

    #endregion

    #region Public Properties

    public Weapon Weapon => _weapon;

    #endregion

    #region Unity Loops

    public virtual void Start()
    {
        //do nothing
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

    private IEnumerator PlayShootFX()
    {
        if(_muzzleFlash != null) { _muzzleFlash.SetActive(true); }

        yield return new WaitForSeconds(0.2f);

        if (_muzzleFlash != null) { _muzzleFlash.SetActive(false); }
    }
}