using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;
using Object = UnityEngine.Object;

[TestFixture]
public class playerJoinManager_tests
{
    private PlayerJoinManager _playerJoinManager;
    private PlayerInputHandler _startingPlayer;
    private PlayerJoinNPC _joinNpc;
    private bool _sceneLoaded = false;

    [OneTimeSetUp]
    public void Setup()
    {
        _sceneLoaded = false;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("PlayTests", LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        _playerJoinManager = GameObject.FindObjectOfType<PlayerJoinManager>();
        GameObject joinLocationPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/PlayerJoinLocation.prefab", typeof(Object));
        GameObject joinLocationInstance = GameObject.Instantiate(joinLocationPrefab, new Vector3(0f, 6f, -2f), Quaternion.identity);
        _joinNpc = GameObject.FindObjectOfType<PlayerJoinNPC>();
        _startingPlayer = GameObject.FindObjectOfType<PlayerInputHandler>();
        _sceneLoaded = true;
    }

    private GameObject CreatePlayerPrefab()
    {
        GameObject player = new GameObject("PlayerTestObj");
        player.AddComponent<PlayerInputHandler>();

        return player;
    }

    //[UnityTest]
    //public IEnumerator check_if_spawnPlayer_spawns_with_correct_settings()
    //{
    //    GameObject playerInstance = _playerJoinManager.SpawnPlayerWithNoInput(0);

    //    Assert.IsNotNull(playerInstance, "Player did not instantiate");

    //    PlayerInputHandler playerInputHandler = playerInstance.GetComponent<PlayerInputHandler>();

    //    Assert.IsTrue(playerInputHandler.CanPlayerSpawn, "CanPlayerSpawn was false");

    //    yield return null;
    //}


    //[UnityTest]
    //public IEnumerator check_if_playerJoinAnimation_disables_player_movement()
    //{
    //    PlayerInputHandler playerInstance = _playerJoinManager.SpawnPlayerWithNoInput(0).GetComponent<PlayerInputHandler>();
    //    PlayerInputHandler playerInstance2 = _playerJoinManager.SpawnPlayerWithNoInput(1).GetComponent<PlayerInputHandler>();

    //    yield return new WaitForEndOfFrame();
    //    yield return new WaitForEndOfFrame();
    //    yield return new WaitForEndOfFrame();

    //    _playerJoinManager.HandleSpawn(playerInstance);
    //    _playerJoinManager.AmountOfActivePlayers = 2;

    //    Assert.IsTrue(playerInstance.IsPlayerActive == false, "IsPlayerActive didn't get set to false");
    //    Assert.IsTrue(playerInstance.CanPlayerSpawn == false, "CanPlayerSpawn didn't get set to false");

    //    yield return null;
    //}

    [UnityTest]
    public IEnumerator check_if_findCorrectNpc_returns_index()
    {
        while (!_sceneLoaded) { yield return null; }

        while (_startingPlayer.AutoRunToLocation(_joinNpc.transform.position))
        {
            yield return new WaitForEndOfFrame();
        }

        _startingPlayer.Interact(true);

        Assert.IsTrue(_playerJoinManager.PlayerJoinNPCs.Length > 0, $"PlayerJoinNPCs was null ; count: {_playerJoinManager.PlayerJoinNPCs.Length}");

        int npcIndex =_playerJoinManager.FindCorrectPlayerJoinNPC();

        Assert.IsTrue(npcIndex > -1, $"did not find npcIndex ; result: {npcIndex}");

        yield return null;
    }

    [UnityTest]
    public IEnumerator check_if_playerJoinAnimation_teleports_player_correctly()
    {
        while (!_sceneLoaded) { yield return null; }

        while (_startingPlayer.AutoRunToLocation(_joinNpc.transform.position)){ yield return new WaitForEndOfFrame(); }

        _startingPlayer.Interact(true);

        GameObject playerInstance = _playerJoinManager.SpawnPlayerWithNoInput(1);
        PlayerInputHandler playerInput = playerInstance.GetComponentInChildren<PlayerInputHandler>();

        yield return new WaitForEndOfFrame();

        playerInput.Jump(true);

        yield return new WaitForSeconds(1f);

        Debug.Log("Do Checks");

        Assert.IsTrue(playerInput.IsPlayerActive == false, "IsPlayerActive didn't get set to false");
        Assert.IsTrue(playerInput.CanPlayerSpawn == false, "CanPlayerSpawn didn't get set to false");

        yield return new WaitForSeconds(4f);

       // _playerJoinManager.HandleSpawn(playerInstance);

        //_playerJoinManager.AmountOfActivePlayers = 2;
        //_playerJoinManager.TransportToSpawnLocation(playerInstance);

        yield return null;
    }

    [TearDown]
    public void Teardown()
    {
        //PlayerInputHandler[] players = GameObject.FindObjectsOfType<PlayerInputHandler>();

        //foreach (PlayerInputHandler player in players)
        //{
        //    GameObject.DestroyImmediate(player.gameObject);
        //}

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
