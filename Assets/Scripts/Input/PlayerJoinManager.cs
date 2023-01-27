using Rewired;
using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerJoinManager : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform _spawnPos;

    #endregion

    #region Private Variables

    private List<PlayerInputHandler> _playerInputs = new List<PlayerInputHandler>();
    private PlayerJoinNPC[] _playerJoinNPC;

    private int _playerJoinNPCIndex = -1;
    [SerializeField] private int _amountOfPlayersActive = 0;

    #endregion

    #region Unity Loops

    private void Start()
    {
        _playerJoinNPC = FindObjectsOfType<PlayerJoinNPC>();
        _playerInputs = FindObjectsOfType<PlayerInputHandler>().ToList<PlayerInputHandler>();
    }

    private void OnDestroy()
    {
        foreach (PlayerInputHandler playerInput in _playerInputs)
        {
            playerInput.OnTrySpawn -= HandleSpawn;
            playerInput.OnJump -= HandleJump;
        }
    }

    #endregion

    public void SpawnPlayer(Player playerInputs, int playerID)
    {
        GameObject player = Instantiate(_playerPrefab, _spawnPos.position, Quaternion.identity);
        PlayerInputHandler playerInput = player.GetComponentInChildren<PlayerInputHandler>();
        _playerInputs.Add(playerInput);
        playerInput.OnTrySpawn += HandleSpawn;
        playerInput.OnJump += HandleJump;
        playerInput.CanPlayerSpawn = true;
        playerInput.PlayerId = playerID;
        playerInput.PlayerInputs = playerInputs;
    }

    private void HandleSpawn(PlayerInputHandler playerInput)
    {
        if (!playerInput.CanPlayerSpawn) { return; }

        _playerJoinNPCIndex = FindCorrectPlayerJoinNPC();

        if (_playerJoinNPCIndex < 0) { return; }

        FindAmountOfPlayersActive();

        TransportToSpawnLocation(playerInput);

    }

    private void TransportToSpawnLocation(PlayerInputHandler playerInput)
    {
        if (_amountOfPlayersActive == 2)
        {
            StartCoroutine(PlayerJoinAnimation(0, playerInput));
        }
        else if (_amountOfPlayersActive == 3)
        {
            StartCoroutine(PlayerJoinAnimation(1, playerInput));
        }
        else if (_amountOfPlayersActive == 4)
        {
            StartCoroutine(PlayerJoinAnimation(2, playerInput));
        }
    }

    private void FindAmountOfPlayersActive()
    {
        _amountOfPlayersActive = 0;
        for (int i = 0; i < _playerInputs.Count; i++)
        {
            if(_playerInputs[i].IsPlayerActive)
            {
                _amountOfPlayersActive++;
            }
        }
    }

    private int FindCorrectPlayerJoinNPC()
    {
        for (int i = 0; i < _playerJoinNPC.Length; i++)
        {
            if (_playerJoinNPC[i].CurrentPlayer != null)
            {
                return i;
            }
        }
        return -1;
    }
    private IEnumerator PlayerJoinAnimation(int index, PlayerInputHandler playerInput)
    {
        int indexAux = index;
        _playerInputs[_playerJoinNPCIndex].IsPlayerActive = false;
        playerInput.CanPlayerSpawn = false;
        _playerJoinNPC[_playerJoinNPCIndex].PlayerJoinTimelines[indexAux].Play();

        yield return new WaitForSeconds(0.5f);

        _playerJoinNPC[_playerJoinNPCIndex].SteamParticles[indexAux].Play();

        yield return new WaitForSeconds(2.5f);

        playerInput.transform.position = _playerJoinNPC[_playerJoinNPCIndex].SpawnLocations[indexAux].transform.position;

        yield return new WaitForSeconds(1);

        _playerJoinNPC[_playerJoinNPCIndex].SteamParticles[indexAux].Stop();
        playerInput.IsPlayerActive = true;
        _playerInputs[_playerJoinNPCIndex].IsPlayerActive = true;
        CameraManager.Instance.SplitScreen(index);
    }

    private void HandleJump()
    {
        for (int i = 0; i < _playerInputs.Count; i++)
        {
            if (!_playerInputs[i].IsPlayerActive && !_playerInputs[i].CanPlayerSpawn)
            {
                _playerInputs[i].IsPlayerActive = true;
            }
        }
    }
}