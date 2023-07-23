using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHeadID : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private int _weaponId = -1;

    #endregion

    #region Private Variables

    private Weapon[] _weaponInteractables;
    private WeaponShoot[] _weaponHeads;

    #endregion

    //Sets all WeaponShoots "_weapon" parameter
    private void Awake()
    {
        _weaponInteractables = transform.root.GetComponentsInChildren<Weapon>(true);
        _weaponHeads = GetComponentsInChildren<WeaponShoot>(true);

        UpdateWeaponId();
    }

    public WeaponHeadID SwapWeaponId()
    {
        _weaponId = Mathf.Clamp(_weaponId + 1, 1, 3);

        if (_weaponId == 3) { _weaponId = 1; }

        Weapon weapon = UpdateWeaponId();

        return weapon.WeaponHeadIdObj;
    }

    private Weapon UpdateWeaponId()
    {
        Weapon wantedWeaponInteractable = null;

        foreach (Weapon weapon in _weaponInteractables)
        {
            if (weapon.WeaponId != _weaponId) { continue; }

            wantedWeaponInteractable = weapon;
        }

        foreach (WeaponShoot weapon in _weaponHeads)
        {
            weapon.SetWeaponInteractable(wantedWeaponInteractable);
        }

        return wantedWeaponInteractable;
    }
}
