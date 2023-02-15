using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
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

    private UpgradeChip[] _upgradeSockets = { null, null };
    private List<GameObject> _chipInstances = new List<GameObject>();
    private InteractableHealth _health;
    protected Upgrade _selectedUpgrade;

    #endregion

    #region Public Properties

    public event Action<Upgrade> OnUpgradeMesh;
    public UpgradeChip[] UpgradeSockets => _upgradeSockets;
    public float? CurrentHealth => _health?.CurrentHealth;

    #endregion

    public override void Awake()
    {
        base.Awake();

        _health = GetComponent<InteractableHealth>();
    }

    public virtual void Start() { EnableUpgradeMesh(); }

    public void LoadChips(UpgradeChip[] allChips, SaveData.UpgradableData upgradableData, ShipData shipData, bool isBooster)
    {
        foreach (UpgradeChip chip in allChips)
        {
            if (upgradableData.Socket1ChipId != chip.Id) { continue; }

            LoadUpgrade(chip, 0);
        }

        foreach (UpgradeChip chip in allChips)
        {
            if (upgradableData.Socket2ChipId != chip.Id) { continue; }

            LoadUpgrade(chip, 1);
        }

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
        if(_upgradeSockets[0] == null && _upgradeSockets[1] == null) { return; }

        InstantiateChipPickups();

        _upgradeSockets[0] = null;
        _upgradeSockets[1] = null;

        foreach (GameObject chip in _chipInstances)
        {
            Destroy(chip.gameObject);
        }
        _chipInstances.Clear();

        EnableUpgradeMesh();
        PlayUpgradeFX();
    }

    public void InstantiateChipPickups()
    {
        foreach (UpgradeChip chip in _upgradeSockets)
        {
            if(chip == null) { continue; }

            GameObject chipPickupInstance = Instantiate(GameAssetsManager.Instance.ChipPickup, _chipDropTransform.position, Quaternion.identity);
            chipPickupInstance.GetComponent<ChipPickup>().Initialize(chip);
        }
    }

    public bool TryUpgrade(UpgradeChip upgradeChip, PlayerCarryController player)
    {
        //If the interactable is broken, return false
        if(TryGetComponent<InteractableHealth>(out InteractableHealth interactableHealth)) { if (interactableHealth.IsDead()) return false; }

        int socketIndex = -1;
        bool foundEmptySocket = false;

        foreach (UpgradeChip socket in _upgradeSockets)
        {
            socketIndex++;

            if (socket != null) { continue; }

            foundEmptySocket = true;

            break;
        }

        if (!foundEmptySocket) { return false; }

        if (!IsSameMkLevel(upgradeChip, player)) { return false; }

        _upgradeSockets[socketIndex] = upgradeChip;

        Upgrade(upgradeChip, socketIndex);

        return true;
    }

    private bool IsSameMkLevel(UpgradeChip newChip, PlayerCarryController player)
    {
        UpgradeChip firstChip = null;

        foreach (UpgradeChip socket in _upgradeSockets)
        {
            if (socket != null) { firstChip = socket; break; }
        }

        if(firstChip == null) { return true; }

        if(firstChip.Level == newChip.Level) { return true; }

        StartCoroutine(PlayNotSameLevelAnim(newChip, player));

        return false;
    }

    private IEnumerator PlayNotSameLevelAnim(UpgradeChip newChip, PlayerCarryController player)
    {
        PlaceChip(newChip, 1);
        //Disable and enable chip to give illusion that the player placed it in the upgradable
        player.CurrentSingleObjInstance.SetActive(false);

        yield return new WaitForSeconds(1f);

        GameObject tempChip = _chipInstances[1];
        _chipInstances.Remove(tempChip);
        Destroy(tempChip);
        player.CurrentSingleObjInstance.SetActive(true);
    }

    public void LoadUpgrade(UpgradeChip upgradeChip, int socketIndex)
    {
        _upgradeSockets[socketIndex] = upgradeChip;

        Upgrade(upgradeChip, socketIndex);
    }

    public void Upgrade(UpgradeChip upgradeChip, int socketIndex)
    {
        if(upgradeChip == null) { return; }

        PlaceChip(upgradeChip, socketIndex);
        EnableUpgradeMesh();
        PlayUpgradeFX();
    }

    private void PlayUpgradeFX()
    {
        GameObject particlesPrefab = GameAssetsManager.Instance.UpgradeParticles;
        Instantiate(particlesPrefab, transform.position, particlesPrefab.transform.rotation);
    }

    private void PlaceChip(UpgradeChip upgradeChip, int socketIndex)
    {
        if(upgradeChip == null) { return; }

        GameObject newChip = Instantiate(upgradeChip.ItemPrefab, _upgradeSocketObjs[socketIndex].transform);
        newChip.transform.localPosition = new Vector3(-0.025f, 0.15f, 0f);
        newChip.transform.localEulerAngles = new Vector3(90f, 0f,-90f);
        _chipInstances.Add(newChip);
    }

    public void EnableUpgradeMesh()
    {
        foreach (Upgrade upgrade in _upgrades)
        {
            upgrade.UpgradeMesh.SetActive(false);
        }

        ChipType socket_1_chip_type = _upgradeSockets[0] ? _upgradeSockets[0].ChipType : ChipType.None;
        ChipType socket_2_chip_type = _upgradeSockets[1] ? _upgradeSockets[1].ChipType : ChipType.None;

        int upgradeMeshIndex = -1;
        foreach (Upgrade upgrade in _upgrades)
        {
            upgradeMeshIndex++;
            if(upgrade._socket_1_ChipType == socket_1_chip_type && upgrade._socket_2_ChipType == socket_2_chip_type)
            {
                break;
            }
        }

        _selectedUpgrade = _upgrades[upgradeMeshIndex];
        GameObject newMesh = _upgrades[upgradeMeshIndex].UpgradeMesh;
        GameObject newProjectile = _upgrades[upgradeMeshIndex].Projectile;
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
    public GameObject Projectile;
    public GameObject[] ShootTransform;
    public ChipType _socket_1_ChipType;
    public ChipType _socket_2_ChipType;
}

#endregion