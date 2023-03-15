using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShoot : MonoBehaviour
{
    #region Private Variable

    protected Weapon _weapon;
    protected float _timeSinceLastShot;

    #endregion

    #region

    [SerializeField] protected float _timeBetweenShots = 0.2f;

    #endregion

    #region Unity Loops

    public virtual void Start()
    {
        _weapon = GetComponentInParent<Weapon>();
    }

    public virtual void Update()
    {
        UpdateTime();
    }

    #endregion

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
        GameObject projectileInstance = Instantiate(_weapon.ProjectilePrefab, _weapon.ShootTransforms[0].position, _weapon.TurretHead.rotation);
        projectileInstance.GetComponent<Projectile>().WeaponReference = _weapon;
    }

    public void ProjectileShootFromOtherBarrels(int shootNumber)
    {
        GameObject projectileInstance = Instantiate(_weapon.ProjectilePrefab, _weapon.ShootTransforms[shootNumber].position, _weapon.TurretHead.rotation);
        projectileInstance.GetComponent<Projectile>().WeaponReference = _weapon;
    }

    public void UpdateTime()
    {
        _timeSinceLastShot += Time.deltaTime;
    }
}