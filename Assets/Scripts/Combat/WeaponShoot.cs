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
        InstantiateProjectiles(_weapon.ShootTransforms[0]);
    }

    public void ProjectileShootFromOtherBarrels(int shootNumber)
    {
        InstantiateProjectiles(_weapon.ShootTransforms[shootNumber]);
    }

    private void InstantiateProjectiles(Transform transform)
    {
        GameObject projectileInstance = Instantiate(_weapon.ProjectilePrefab, transform.position, _weapon.TurretHead.rotation);
        projectileInstance.GetComponent<Projectile>().WeaponReference = _weapon;
    }

    public void UpdateTime()
    {
        _timeSinceLastShot += Time.deltaTime;
    }
}