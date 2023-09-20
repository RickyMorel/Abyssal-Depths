using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SaveData;

public class ShipData : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private bool _loadData = false;
    [SerializeField] private Transform[] _playerSpawnPositions;
    [SerializeField] private int _currentSaveIndex = 0;

    #endregion

    #region Getters & Setters

    public string ShipName { get; private set; }
    public string Location { get; private set; }
    public float PlayTime { get; private set; }
    public int CurrentSaveIndex => _currentSaveIndex;
    public bool LoadData => _loadData;
    public SaveData.LevelData[] LevelData => GameManager.Instance.LevelData.LevelDataArray;

    #endregion

    #region Public Properties

    [Header("Upgradables")]
    public Upgradable Booster;
    public Upgradable[] Weapons;

    #endregion

    #region Unity Loops

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        GameObject loadIndexObj = GameObject.FindGameObjectWithTag("LoadIndex");

#if UNITY_EDITOR
        //if (!_loadData) { return; }

        //else if(loadIndexObj == null)
        //{
        //    _currentSaveIndex = 0;
        //   // StartCoroutine(LateStart()); 
        //}
#endif

        if (loadIndexObj == null) { return; }

        string loadIndexObjName = loadIndexObj.name;

        string[] splitString = loadIndexObjName.Split(":");

        _currentSaveIndex = int.Parse(splitString[1]);

       // StartCoroutine(LateStart());
    }

    //private IEnumerator LateStart()
    //{
    //    yield return new WaitForEndOfFrame();

    //    SaveData saveData = SaveSystem.Load(_currentSaveIndex);

    //    if(saveData != null)
    //    {
    //        SetFileData(saveData);
    //        LoadMinables(saveData);
    //        LoadEnemies(saveData);
    //        LoadInventories(saveData);
    //        LoadChips(saveData);
    //        TryLoadDeathLoot(saveData);
    //        GameManager.Instance.LevelData.LoadLevelData(saveData); 
    //    }
    //    else
    //    {
    //        transform.position = GameObject.FindGameObjectWithTag(GameTagsManager.SPAWN_POINT).transform.position;
    //    }
    //}

    private void Update()
    {
        PlayTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.K)) { SaveSystem.Save(_currentSaveIndex); }
    }

    #endregion

    public void ReloadLevel()
    {
        //SaveData saveData = SaveSystem.Load(_currentSaveIndex);

        //LoadInventories(saveData);
        //LoadChips(saveData, GameObject.FindGameObjectWithTag(GameTagsManager.SPAWN_POINT).transform.position);
        //TryLoadDeathLoot(saveData);

        //Booster.TrySetHealth(int.MaxValue, this, true);

        //for (int i = 0; i < Weapons.Length; i++)
        //{
        //    Weapons[i].TrySetHealth(int.MaxValue, this, false);
        //}
    }

    public void SetFileData(SaveData saveData)
    {
        ShipName = saveData.ShipName;
        if (Location == string.Empty) { Location = saveData.LocationName; }
        if (PlayTime < 1) { PlayTime = saveData.PlayedTime; }
    }

    public void SetLocation(string newLocation)
    {
        Location = newLocation;
    }

    private void TryLoadDeathLoot(SaveData saveData)
    {
        if(saveData._deathLootData == null) { return; }

        GameObject deathLootInstance = Instantiate(GameAssetsManager.Instance.DeathLootPickup);
        DeathLoot deathLoot = deathLootInstance.GetComponent<DeathLoot>();
        deathLoot.LoadSavedItems(saveData._deathLootData.Items);
        Vector3 savedPos = new Vector3(saveData._deathLootData.Position[0], saveData._deathLootData.Position[1], saveData._deathLootData.Position[2]);
        deathLoot.transform.position = savedPos;
    }

    private void LoadInventories(SaveData saveData)
    {
        if (saveData._mainInventory != null && saveData._mainInventory.Count > 0)
        {
            MainInventory.Instance.LoadSavedItems(saveData._mainInventory);
        }
        if (saveData._shipInventory != null && saveData._shipInventory.Count > 0)
        {
            ShipInventory.Instance.LoadSavedItems(saveData._shipInventory);
        }
    }

    private void LoadChips(SaveData saveData, Vector3 wantedSpawnPosition = default(Vector3))
    {
        Booster.LoadChips(saveData.BoosterData, this, true);

        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapons[i].LoadChips(saveData.WeaponDatas[i], this, false);
        }

        transform.position = GameObject.FindGameObjectWithTag(GameTagsManager.SPAWN_POINT).transform.position;

        SpawnPlayers();
    }

    private void LoadMinables(SaveData saveData)
    {
        Minable[] currentMinables = FindObjectsOfType<Minable>();

        foreach (Minable minable in currentMinables)
        {
            MinableData wantedMinable = saveData._minablesInScene.Find(x => x.Id == minable.Id);

            if (wantedMinable == null) { return; }

            minable.gameObject.SetActive(wantedMinable.IsActive);
        }
    }

    private void LoadEnemies(SaveData saveData)
    {
        if (saveData.enemiesInScene == null || saveData.enemiesInScene.Count < 1) { return; }

        AIStateMachine[] currentEnemies = FindObjectsOfType<AIStateMachine>();

        foreach (AIStateMachine currentEnemy in currentEnemies)
        {
            SaveData.EnemyData wantedEnemyData = saveData.enemiesInScene.Find(x => x.EnemyId == currentEnemy.Id);

            if (wantedEnemyData == null) { continue; }

            currentEnemy.GetComponent<AIHealth>().SetHealth((int)wantedEnemyData.Health);
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

    public AIStateMachine[] GetCurrentEnemyData()
    {
        return FindObjectsOfType<AIStateMachine>(true);
    }

    public Minable[] GetCurrentMinableData()
    {
        return FindObjectsOfType<Minable>(true);
    }

    public DeathLoot GetCurrentDeathLoot()
    {
        return FindObjectOfType<DeathLoot>();
    }
}
