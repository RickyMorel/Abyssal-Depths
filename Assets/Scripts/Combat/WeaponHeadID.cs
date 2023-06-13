using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHeadID : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private int _weaponId = -1;

    #endregion

    //Sets all WeaponShoots "_weapon" parameter
    private void Awake()
    {
        Weapon[] weaponInteractables = transform.root.GetComponentsInChildren<Weapon>(true);
        Weapon wantedWeaponInteractable = null;

        foreach (Weapon weapon in weaponInteractables)
        {
            if(weapon.WeaponId != _weaponId) { continue; }

            wantedWeaponInteractable = weapon;
        }

        WeaponShoot[] weaponHeads = GetComponentsInChildren<WeaponShoot>(true);

        foreach (WeaponShoot weapon in weaponHeads)
        {
            weapon.SetWeaponInteractable(wantedWeaponInteractable);
        }
    }
}
