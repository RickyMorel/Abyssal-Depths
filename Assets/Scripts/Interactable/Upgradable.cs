using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Playables;
using static SaveData;

public class Upgradable : Interactable
{
    #region Editor Fields

    [Header("Upgrades")]
    [SerializeField] private GameObject[] _upgradeSocketObjs;
    [SerializeField] private Upgrade[] _upgrades;
    [SerializeField] private Transform _chipDropTransform;

    #endregion

    #region Private Variables

    private UpgradableHumble _humbleUpgradable;

    private InteractableHealth _health;
    protected Upgrade _selectedUpgrade;
    protected int _chipLevel;

    #endregion

    #region Public Properties

    public event Action<Upgrade> OnUpgradeMesh;
    public UpgradeChip[] UpgradeSockets => _humbleUpgradable.UpgradeSockets;
    public float? CurrentHealth => _health?.CurrentHealth;
    public int ChipLevel => _chipLevel;

    #endregion

    public override void Awake()
    {
        base.Awake();

        _health = GetComponent<InteractableHealth>();

        _humble = new UpgradableHumble(this, InteractableHealth, IsAIOnlyInteractable);
        _humbleUpgradable = _humble as UpgradableHumble;
    }

    public virtual void Start() { EnableUpgradeMesh(); }

    public void LoadChips(UpgradableData upgradableData, ShipData shipData, bool isBooster)
    {
        UpgradeChip upgradeChip_1 = SaveUtils.GetChipById(upgradableData.Socket1ChipId);

        if(upgradeChip_1 != null) { LoadUpgrade(upgradeChip_1, 0); }

        UpgradeChip upgradeChip_2 = SaveUtils.GetChipById(upgradableData.Socket2ChipId);

        if (upgradeChip_2 != null) { LoadUpgrade(upgradeChip_2, 1); }

        TrySetHealth((int)upgradableData.CurrentHealth, shipData, isBooster);
    }

    public void TrySetHealth(int wantedHealth, ShipData shipData, bool isBooster)
    {
        if (!isBooster) { _health.SetHealth(wantedHealth); }
        else { shipData.GetComponent<ShipHealth>().SetHealth(wantedHealth); }

        StartCoroutine(WaitForLoadFix(wantedHealth));
    }

    private IEnumerator WaitForLoadFix(int wantedHealth)
    {
        yield return new WaitForEndOfFrame();

        if (wantedHealth > 0) { _health.FixInteractable(false); }
    }

    public void RemoveUpgrades()
    {
        InstantiateChipPickups();

        if (_humbleUpgradable.RemoveUpgrades() == false) { return; }

        EnableUpgradeMesh();
        PlayUpgradeFX();
    }

    public void InstantiateChipPickups()
    {
        foreach (UpgradeChip chip in _humbleUpgradable.UpgradeSockets)
        {
            if (chip == null) { continue; }

            GameObject chipPickupInstance = GameObject.Instantiate(GameAssetsManager.Instance.ChipPickup, _chipDropTransform.position, Quaternion.identity);
            chipPickupInstance.GetComponent<ChipPickup>().Initialize(chip);
        }
    }

    public bool TryUpgrade(UpgradeChip upgradeChip, PlayerCarryController player)
    {
        _chipLevel = upgradeChip.Level;

        //If the interactable is broken, return false
        if(TryGetComponent(out InteractableHealth interactableHealth)) { if (interactableHealth.IsDead()) return false; }

        int socketIndex = -1;
        bool foundEmptySocket = false;

        foreach (UpgradeChip socket in _humbleUpgradable.UpgradeSockets)
        {
            socketIndex++;

            if (socket != null) { continue; }

            foundEmptySocket = true;

            break;
        }

        if (!foundEmptySocket) { return false; }

        if (!_humbleUpgradable.IsSameMkLevel(upgradeChip)) { StartCoroutine(PlayNotSameLevelAnim(upgradeChip, player)); return false; }

        LoadUpgrade(upgradeChip, socketIndex);

        return true;
    }

    private IEnumerator PlayNotSameLevelAnim(UpgradeChip newChip, PlayerCarryController player)
    {
        if (TryGetComponent(out PlayableDirector playableDirector)) { playableDirector.Play(); }

        _humbleUpgradable.PlaceChip(newChip, _upgradeSocketObjs, 1);
        //Disable and enable chip to give illusion that the player placed it in the upgradable
        player.CurrentSingleObjInstance.SetActive(false);

        yield return new WaitForSeconds(1f);

        GameObject tempChip = _humbleUpgradable.ChipInstances[1];
        _humbleUpgradable.ChipInstances.Remove(tempChip);
        Destroy(tempChip);
        player.CurrentSingleObjInstance.SetActive(true);
    }

    public void LoadUpgrade(UpgradeChip upgradeChip, int socketIndex)
    {
        if(!_humbleUpgradable.LoadUpgrade(upgradeChip, socketIndex)) { return; }

        Upgrade(upgradeChip, socketIndex);
    }

    public void Upgrade(UpgradeChip upgradeChip, int socketIndex)
    {
        if(upgradeChip == null) { return; }

        _humbleUpgradable.PlaceChip(upgradeChip, _upgradeSocketObjs, socketIndex);
        EnableUpgradeMesh();
        PlayUpgradeFX();
    }

    private void PlayUpgradeFX()
    {
        GameObject particlesPrefab = GameAssetsManager.Instance.UpgradeParticles;
        Instantiate(particlesPrefab, transform.position, particlesPrefab.transform.rotation);
    }

    public void EnableUpgradeMesh()
    {
        foreach (Upgrade upgrade in _upgrades)
        {
            upgrade.UpgradeMesh.SetActive(false);
        }

        ChipType socket_1_chip_type = _humbleUpgradable.UpgradeSockets[0] ? _humbleUpgradable.UpgradeSockets[0].ChipType : ChipType.None;
        ChipType socket_2_chip_type = _humbleUpgradable.UpgradeSockets[1] ? _humbleUpgradable.UpgradeSockets[1].ChipType : ChipType.None;

        int upgradeMeshIndex = -1;
        foreach (Upgrade upgrade in _upgrades)
        {
            upgradeMeshIndex++;
            if(upgrade.UpgradeSO.Socket_1 == socket_1_chip_type && upgrade.UpgradeSO.Socket_2 == socket_2_chip_type)
            {
                break;
            }
        }

        _selectedUpgrade = _upgrades[upgradeMeshIndex];
        GameObject newMesh = _upgrades[upgradeMeshIndex].UpgradeMesh;
        GameObject newProjectile = _upgrades[upgradeMeshIndex].UpgradeSO.ProjectilePrefab;
        GameObject[] newShootTransform = _upgrades[upgradeMeshIndex].ShootTransform;

        newMesh.SetActive(true);

        OnUpgradeMesh?.Invoke(_upgrades[upgradeMeshIndex]);
    }
}

#region Helper Classes

[System.Serializable]
public class Upgrade
{
    public GameObject UpgradeMesh;
    public GameObject[] ShootTransform;
    public WeaponSO UpgradeSO;
}

#endregion