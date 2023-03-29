using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerJoinNPC : NPC
{
    #region Editor Fields

    [SerializeField] private GameObject _playersToJoinText;

    [SerializeField] private PlayableDirector[] _playerJoinTimelines;

    [SerializeField] private ParticleSystem[] _steamParticles;

    [SerializeField] private Transform[] _spawnLocations;

    #endregion

    #region Private Variables

    private bool _canPlayersJoin;

    #endregion

    #region Public Properties

    public PlayableDirector[] PlayerJoinTimelines => _playerJoinTimelines;

    public ParticleSystem[] SteamParticles => _steamParticles;

    public Transform[] SpawnLocations => _spawnLocations;

    public bool CanPlayersJoin => _canPlayersJoin;

    #endregion

    #region Unity Loops

    public void Update()
    {
        DisplayUI();
    }

    #endregion

    private void DisplayUI()
    {
        if (CurrentPlayer == null)
        {
            _playersToJoinText.SetActive(false);
            _canPlayersJoin = false;
        }
        else
        {
            _playersToJoinText.SetActive(true);
            _canPlayersJoin = true;
        }
    }
}
