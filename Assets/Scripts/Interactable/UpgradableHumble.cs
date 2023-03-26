using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradableHumble : InteractableHumble
{
    public UpgradeChip[] UpgradeSockets { get; private set; } = { null, null };
    public List<GameObject> ChipInstances { get; private set; } = new List<GameObject>();

    public UpgradableHumble(Interactable interactable, InteractableHealth interactableHealth, bool isAIOnlyInteractable) : base(interactable, interactableHealth, isAIOnlyInteractable)
    {

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
            GameObject.Destroy(chip.gameObject);
        }

        ChipInstances.Clear();

        return true;
    }

    public bool LoadUpgrade(UpgradeChip upgradeChip, int socketIndex)
    {
        UpgradeSockets[socketIndex] = upgradeChip;

        return true;
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
