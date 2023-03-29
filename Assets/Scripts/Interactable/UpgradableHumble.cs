using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradableHumble : InteractableHumble
{
    public UpgradeChip[] UpgradeSockets { get; private set; } = { null, null };
    public List<GameObject> ChipInstances { get; private set; } = new List<GameObject>();
    public int ChipLevel { get; private set; }

    public UpgradableHumble(bool isAIOnlyInteractable) : base(isAIOnlyInteractable)
    {

    }

    public UpgradableHumble(UpgradeChip[] upgradeSockets, List<GameObject> chipInstances, bool isAIOnlyInteractable) : base(isAIOnlyInteractable)
    {
        UpgradeSockets = upgradeSockets;
        ChipInstances = chipInstances;
    }

    public bool IsSameMkLevel(UpgradeChip newChip)
    {
        UpgradeChip firstChip = null;

        foreach (UpgradeChip socket in UpgradeSockets)
        {
            if (socket != null) { firstChip = socket; break; }
        }

        if (firstChip == null) { return true; }

        if (firstChip.Level == newChip.Level) { return true; }

        return false;
    }

    public bool RemoveUpgrades()
    {
        if (UpgradeSockets[0] == null && UpgradeSockets[1] == null) { return false; }

        UpgradeSockets[0] = null;
        UpgradeSockets[1] = null;

        foreach (GameObject chip in ChipInstances)
        {
            GameObject.DestroyImmediate(chip.gameObject);
        }

        ChipInstances.Clear();

        return true;
    }

    public bool TryUpgrade(UpgradeChip upgradeChip, GameObject interactableObj, out int socketIndex, out bool isSameMKLevel)
    {
        ChipLevel = upgradeChip.Level;
        isSameMKLevel = true;
        socketIndex = -1;

        //If the interactable is broken, return false
        if (interactableObj.TryGetComponent(out InteractableHealth interactableHealth)) { if (interactableHealth.IsDead()) return false; }

        bool foundEmptySocket = false;

        foreach (UpgradeChip socket in UpgradeSockets)
        {
            socketIndex++;

            if (socket != null) { continue; }

            foundEmptySocket = true;

            break;
        }

        if (!foundEmptySocket) { return false; }

        if (!IsSameMkLevel(upgradeChip)) { isSameMKLevel = false; return false; }

        return true;
    }

    public bool LoadUpgrade(UpgradeChip upgradeChip, int socketIndex)
    {
        UpgradeSockets[socketIndex] = upgradeChip;

        return true;
    }

    public int EnableCorrectUpgradeMesh(Upgrade[] upgrades)
    {
        foreach (Upgrade upgrade in upgrades)
        {
            upgrade.UpgradeMesh.SetActive(false);
        }

        ChipType socket_1_chip_type = UpgradeSockets[0] ? UpgradeSockets[0].ChipType : ChipType.None;
        ChipType socket_2_chip_type = UpgradeSockets[1] ? UpgradeSockets[1].ChipType : ChipType.None;

        int upgradeMeshIndex = -1;
        foreach (Upgrade upgrade in upgrades)
        {
            upgradeMeshIndex++;
            if (upgrade.UpgradeSO.Socket_1 == socket_1_chip_type && upgrade.UpgradeSO.Socket_2 == socket_2_chip_type)
            {
                break;
            }
        }

        return upgradeMeshIndex;
    }

    public void PlaceChip(UpgradeChip upgradeChip, GameObject[] upgradeSocketObjs, int socketIndex)
    {
        if (upgradeChip == null) { return; }

        GameObject newChip = GameObject.Instantiate(upgradeChip.ItemPrefab, upgradeSocketObjs[socketIndex].transform);
        newChip.transform.localPosition = new Vector3(-0.025f, 0.15f, 0f);
        newChip.transform.localEulerAngles = new Vector3(90f, 0f, -90f);
        ChipInstances.Add(newChip);
    }
}
