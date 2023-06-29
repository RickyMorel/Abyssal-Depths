using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelData : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private SaveData.LevelData[] _levelDataArray;

    #endregion

    #region Public Properties

    public SaveData.LevelData[] LevelDataArray => _levelDataArray;

    #endregion

    private void Awake()
    {
        if(_levelDataArray.Count() > 0) { return; }

        _levelDataArray = new SaveData.LevelData[SceneManager.sceneCountInBuildSettings];

        for (int i = 0; i < _levelDataArray.Length; i++)
        {
            _levelDataArray[i] = new SaveData.LevelData(i, false, false); 
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
        NextLevelTriggerZone.OnLevelCompleted += HandleLevelCompleted;
        AIHealth.OnBossDied += HandleBossDefeated;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        NextLevelTriggerZone.OnLevelCompleted -= HandleLevelCompleted;
        AIHealth.OnBossDied -= HandleBossDefeated;
    }

    public void LoadLevelData(SaveData saveData)
    {
        _levelDataArray = saveData._levelDataArray;

        HandleSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        SaveData.LevelData levelData = _levelDataArray[scene.buildIndex];

        Item portalPieceItem = FindObjectOfType<GateCrystal>(true).LootList[0].Item;

        MainInventory.Instance.RemoveItemsOfType(portalPieceItem);
        ShipInventory.Instance.RemoveItemsOfType(portalPieceItem);

        if (levelData.IsCompleted)
        {
            GateCrystal[] gateCrystals = FindObjectsOfType<GateCrystal>();

            foreach (GateCrystal crystal in gateCrystals)
            {
                crystal.gameObject.SetActive(false);
            }

            NextLevelTriggerZone nextLevelTriggerZone = FindObjectOfType<NextLevelTriggerZone>();

            nextLevelTriggerZone.PortalPhase4();
        }

        if (levelData.IsBossDefeated)
        {
            BossZone bossZone = FindObjectOfType<BossZone>();

            bossZone.gameObject.SetActive(false);

            MainInventory.Instance.AddItem(new ItemQuantity(portalPieceItem, 1));
        }
    }

    private void HandleLevelCompleted(int sceneBuildIndex)
    {
        _levelDataArray[sceneBuildIndex].IsCompleted = true;
    }

    private void HandleBossDefeated(int sceneBuildIndex)
    {
        _levelDataArray[sceneBuildIndex].IsBossDefeated = true;
    }
}
