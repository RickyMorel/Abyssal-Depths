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
        string socket_1_id = shipData.Booster.UpgradeSockets[0] == null ? " " : shipData.Booster.UpgradeSockets[0].Id;
        string socket_2_id = shipData.Booster.UpgradeSockets[1] == null ? " " : shipData.Booster.UpgradeSockets[1].Id;
        float health = shipData.Booster.CurrentHealth == null ? 0 : shipData.Booster.CurrentHealth.Value;

        BoosterData = new UpgradableData(socket_1_id, socket_2_id, health);

        for (int i = 0; i < shipData.Weapons.Length; i++)
        {
            string socket_w1_id = shipData.Weapons[i].UpgradeSockets[0] == null ? " " : shipData.Weapons[i].UpgradeSockets[0].Id;
            string socket_w2_id = shipData.Weapons[i].UpgradeSockets[1] == null ? " " : shipData.Weapons[i].UpgradeSockets[1].Id;
            float health_w = shipData.Weapons[i].CurrentHealth == null ? 0 : shipData.Weapons[i].CurrentHealth.Value;

            Debug.Log("shipData.Weapons[i].CurrentHealth: " + shipData.Weapons[i].CurrentHealth);
            Debug.Log("health_w: " + health_w);

            WeaponDatas[i] = new UpgradableData(socket_w1_id, socket_w2_id, health_w);
        }
    }

    [System.Serializable]
    public class UpgradableData
    {
        public string Socket1ChipId;
        public string Socket2ChipId;
        public float CurrentHealth;

        public UpgradableData(string socket1ChipId, string socket2ChipId, float health)
        {
            Socket1ChipId = socket1ChipId;
            Socket2ChipId = socket2ChipId;
            CurrentHealth = health;
        }   
    }
}
