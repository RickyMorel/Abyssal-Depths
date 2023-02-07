using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        LoadChips();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) { SaveSystem.Save(); }
    }

    private void LoadChips()
    {
        UpgradeChip[] allChips = Resources.LoadAll<UpgradeChip>("ScriptableObjs/Chips");
        SaveData saveData = SaveSystem.Load();

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
}
