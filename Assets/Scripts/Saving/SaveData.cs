using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public UpgradableData BoosterData;
    public UpgradableData[] WeaponDatas = { null, null, null, null };

    public SaveData(ShipData shipData)
    {
        BoosterData = new UpgradableData(shipData.Booster.UpgradeSockets[0].Id, shipData.Booster.UpgradeSockets[1].Id);

        for (int i = 0; i < shipData.Weapons.Length; i++)
        {
            WeaponDatas[i] = new UpgradableData(shipData.Weapons[i].UpgradeSockets[0].Id, shipData.Weapons[i].UpgradeSockets[1].Id);
        }
    }

    public class UpgradableData
    {
        public string Socket1ChipId;
        public string Socket2ChipId;

        public UpgradableData(string socket1ChipId, string socket2ChipId)
        {
            Socket1ChipId = socket1ChipId;
            Socket2ChipId = socket2ChipId;
        }   
    }
}
