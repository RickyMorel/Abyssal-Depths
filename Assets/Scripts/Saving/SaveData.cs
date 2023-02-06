using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SaveData
{
    public UpgradableData BoosterData;
    public UpgradableData[] WeaponDatas = { null, null, null, null };
    public float[] ShipPos = { 0f, 0f, 0f};
    public int CurrentSceneIndex;

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

            WeaponDatas[i] = new UpgradableData(socket_w1_id, socket_w2_id, health_w);
        }

        ShipPos[0] = shipData.transform.position.x;
        ShipPos[1] = shipData.transform.position.y;
        ShipPos[2] = shipData.transform.position.z;
        CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
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
