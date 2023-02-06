using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipData : MonoBehaviour
{
    public Upgradable Booster;
    public Upgradable[] Weapons;

    private void Start()
    {
        LoadChips();
    }

    private void LoadChips()
    {
        UpgradeChip[] allChips = Resources.LoadAll<UpgradeChip>("ScriptableObjs/Chips");
        SaveData saveData = SaveSystem.Load();

        Booster.LoadChips(allChips, saveData.BoosterData, this, true);

        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapons[i].LoadChips(allChips, saveData.WeaponDatas[i], this, false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) { SaveSystem.Save(); }
    }
}
