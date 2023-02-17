using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void AssignAmmoType(ChipType chipType)
    {
        Debug.Log("AssignAmmoType: " + chipType);
        Color wantedColor = _baseAmmoColor;

        switch (chipType)
        {
            case ChipType.Base:
                wantedColor = _baseAmmoColor;
                break;
            case ChipType.Fire:
                wantedColor = _fireAmmoColor;
                break;
            case ChipType.Electric:
                wantedColor = _electricAmmoColor;
                break;
            case ChipType.Laser:
                wantedColor = _laserAmmoColor;
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
