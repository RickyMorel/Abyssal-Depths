using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgradable : Interactable
{
    #region Editor Fields

    [Header("Upgrades")]
    [SerializeField] private GameObject[] _upgradeSocketObjs;
    [SerializeField] private Upgrade[] _upgrades;
    [SerializeField] private Transform _chipDropTransform;

    [Header("FX")]
    [SerializeField] private ParticleSystem _upgradeParticles;

    #endregion

    #region Private Variables

    private UpgradeChip[] _upgradeSockets = { null, null };
    private List<GameObject> _chipInstances = new List<GameObject>();

    #endregion

    #region Public Properties

    public event Action<GameObject> OnUpgradeMesh;

    #endregion

    #region Unity Loops

    public virtual void Start()
    {
        //TODO: read current level from save data

        EnableUpgradeMesh();
    }

    #endregion

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

    public bool TryUpgrade(UpgradeChip upgradeChip)
    {
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

        _upgradeSockets[socketIndex] = upgradeChip;

        Upgrade(upgradeChip, socketIndex);

        return true;
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
        _upgradeParticles.Play();
    }

    private void PlaceChip(UpgradeChip upgradeChip, int socketIndex)
    {
        if(upgradeChip == null) { return; }

        GameObject newChip = Instantiate(upgradeChip.Prefab, _upgradeSocketObjs[socketIndex].transform);
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

        GameObject newMesh = _upgrades[upgradeMeshIndex].UpgradeMesh;

        newMesh.SetActive(true);

        OnUpgradeMesh?.Invoke(newMesh);
    }
}

#region Helper Classes

[System.Serializable]
public class Upgrade
{
    public GameObject UpgradeMesh;
    public ChipType _socket_1_ChipType;
    public ChipType _socket_2_ChipType;
}

#endregion
