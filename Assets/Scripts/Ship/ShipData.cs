using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipData : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private Transform[] _playerSpawnPositions;

    #endregion

    #region Public Properties

    public Upgradable Booster;
    public Upgradable[] Weapons;

    #endregion

    private void Start()
    {
        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();

        SaveData saveData = SaveSystem.Load();

        LoadChips(saveData);
        LoadEnemies(saveData);
        LoadInventories(saveData);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) { SaveSystem.Save(); }
    }

    private void LoadInventories(SaveData saveData)
    {
        if(saveData._mainInventory != null && saveData._mainInventory.Count > 0)
        {
            MainInventory.Instance.LoadSavedItems(saveData._mainInventory);
        }
    }

    private void LoadChips(SaveData saveData)
    {
        UpgradeChip[] allChips = Resources.LoadAll<UpgradeChip>("ScriptableObjs/Chips");

        //SceneManager.LoadScene(saveData.CurrentSceneIndex);

        Booster.LoadChips(allChips, saveData.BoosterData, this, true);

        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapons[i].LoadChips(allChips, saveData.WeaponDatas[i], this, false);
        }

        Vector3 spawnPos = new Vector3(saveData.ShipPos[0], saveData.ShipPos[1], saveData.ShipPos[2]);
        transform.position = spawnPos;

        SpawnPlayers();
    }

    private void LoadEnemies(SaveData saveData)
    {
        if(saveData.enemiesInScene == null || saveData.enemiesInScene.Count < 1) { return; }

        AIHealth[] currentEnemies = FindObjectsOfType<AIHealth>();

        foreach (AIHealth currentEnemy in currentEnemies)
        {
            SaveData.EnemyData wantedEnemyData = saveData.enemiesInScene.Find(x => x.EnemyId == currentEnemy.Id);

            if(wantedEnemyData == null) { continue; }

            currentEnemy.SetHealth((int)wantedEnemyData.Health);
            Vector3 savedPos = new Vector3(wantedEnemyData.Position[0], wantedEnemyData.Position[1], wantedEnemyData.Position[2]);
            currentEnemy.transform.position = savedPos;
        }
    }

    private void SpawnPlayers()
    {
        PlayerComponents[] players = FindObjectsOfType<PlayerComponents>();

        int spawnIndex = 0;
        foreach (PlayerComponents player in players)
        {
            player.transform.position = _playerSpawnPositions[spawnIndex].transform.position;
            spawnIndex = Mathf.Clamp(spawnIndex++, 0, _playerSpawnPositions.Count() - 1);
        }
    }

    public AIHealth[] GetCurrentEnemyData()
    {
        return FindObjectsOfType<AIHealth>(true);
    }
}
