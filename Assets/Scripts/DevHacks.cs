using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevHacks : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject _gateKeyPickupPrefab;

    #endregion

    #region Private Variables

    private Weapon[] _shipWeapons;
    private UpgradeChip _baseChipMK1, _fireChipMK1, _electricChipMK1, _laserChipMK1;

    #endregion

    private void Start()
    {
        _shipWeapons = FindObjectsOfType<Weapon>();
        _baseChipMK1 = Resources.Load<UpgradeChip>("ScriptableObjs/Chips/BaseChipMK1");
        _fireChipMK1 = Resources.Load<UpgradeChip>("ScriptableObjs/Chips/FireChipMK1");
        _electricChipMK1 = Resources.Load<UpgradeChip>("ScriptableObjs/Chips/ElectricChipMK1");
        _laserChipMK1 = Resources.Load<UpgradeChip>("ScriptableObjs/Chips/LaserChipMK1");
    }

    private void Update()
    {
        DayNightHacks();
        UpgradeHacks();
        if (Input.GetKeyDown(KeyCode.K)) { Instantiate(_gateKeyPickupPrefab, ShipMovingStaticManager.Instance.transform.position, Quaternion.identity); }
    }

    private void UpgradeHacks()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            RemoveUpgrades();
            _shipWeapons[0].TryUpgrade(_laserChipMK1, null);
            _shipWeapons[1].TryUpgrade(_electricChipMK1, null);
            _shipWeapons[1].TryUpgrade(_electricChipMK1, null);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RemoveUpgrades();
            _shipWeapons[0].TryUpgrade(_laserChipMK1, null);
            _shipWeapons[0].TryUpgrade(_laserChipMK1, null);
            _shipWeapons[1].TryUpgrade(_fireChipMK1, null);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            RemoveUpgrades();
            _shipWeapons[0].TryUpgrade(_baseChipMK1, null);
            _shipWeapons[0].TryUpgrade(_laserChipMK1, null);
            _shipWeapons[1].TryUpgrade(_fireChipMK1, null);
            _shipWeapons[1].TryUpgrade(_baseChipMK1, null);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            RemoveUpgrades();
            _shipWeapons[0].TryUpgrade(_baseChipMK1, null);
            _shipWeapons[1].TryUpgrade(_electricChipMK1, null);
        }
    }

    private void DayNightHacks()
    {
        if (DayNightManager.Instance.ActivateTimer) { DayNightManager.Instance.BrightnessLerps(); }

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (DayNightManager.Instance.IsNightTime)
            {
                DayNightManager.Instance.DayEffectsTransition();
            }
            else
            {
                DayNightManager.Instance.NightEffectsTransition();
            }
        }

        if (Input.GetKeyDown(KeyCode.J)) { Time.timeScale = 20f; }
    }

    private void RemoveUpgrades()
    {
        _shipWeapons[0].RemoveUpgrades(false);
        _shipWeapons[1].RemoveUpgrades(false);
    }
}
