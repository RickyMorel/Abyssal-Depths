using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;

public class AmmoPickup : MonoBehaviour
{
    #region Editor Fields

    [Header("Visuals")]
    [ColorUsage(true, true)]
    public Color _baseAmmoColor;
    [ColorUsage(true, true)]
    public Color _fireAmmoColor;
    [ColorUsage(true, true)]
    public Color _electricAmmoColor;
    [ColorUsage(true, true)]
    public Color _laserAmmoColor;

    [SerializeField] private MeshRenderer _renderer;

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.GetComponent<Ship>()) { return; }

        Pickup();
    }

    public void AssignAmmoType(DamageType damageType, Weapon weapon)
    {
        ChipType wantedChipType = weapon.UpgradeSockets[0] != null ? weapon.UpgradeSockets[0].ChipType : ChipType.None;
        Color wantedColor = _baseAmmoColor;

        //if weapon has both chips, pick an ammo type
        if (weapon.UpgradeSockets[1] != null)
        {
            int randomAmmo = Random.Range(0, 2);
            wantedChipType = weapon.UpgradeSockets[randomAmmo].ChipType;
        }

        switch (damageType)
        {
            case DamageType.Base:
                wantedColor = _baseAmmoColor;
                break;
            case DamageType.Fire:
                wantedColor = _fireAmmoColor;
                break;
            case DamageType.Electric:
                wantedColor = _electricAmmoColor;
                break;
            case DamageType.Laser:
                wantedColor = _laserAmmoColor;
                break;
            default:
                //if has no chips, then don't spawn ammo pickup
                Destroy(gameObject);
                break;
        }

        var mat = _renderer.material;
        mat.SetColor("_BaseColor", wantedColor);
        mat.SetColor("_EmissionColor", wantedColor);
    }

    private void Pickup()
    {
        Destroy(gameObject);
    }
}
