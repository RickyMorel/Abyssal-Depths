using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(DeathManager))]
[RequireComponent(typeof(LevelData))]
public class GameManager : MonoBehaviour
{
    #region Public Properties

    public static GameManager Instance { get { return _instance; } }

    public DeathManager DeathManager => _deathManager;
    public LevelData LevelData => _levelData;

    #endregion

    #region Private Variables

    private static GameManager _instance;

    private DeathManager _deathManager;
    private LevelData _levelData;

    #endregion

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        _deathManager = GetComponent<DeathManager>();
        _levelData = GetComponent<LevelData>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Destroy(Ship.Instance.gameObject);

            PlayerInputHandler[] players = FindObjectsOfType<PlayerInputHandler>(true);
            PlayerCamera[] playerCameras = FindObjectsOfType<PlayerCamera>(true);
            AttackHitBox[] attackHitboxes = FindObjectsOfType<AttackHitBox>();

            foreach (var player in players)
            {
                Destroy(player.transform.root.gameObject);
            }

            foreach (var playerCamera in playerCameras)
            {
                Destroy(playerCamera.transform.root.gameObject);
            }

            foreach (var hitbox in attackHitboxes)
            {
                Destroy(hitbox.transform.root.gameObject);
            }

            SceneManager.LoadScene(0);
        }
    }
}
