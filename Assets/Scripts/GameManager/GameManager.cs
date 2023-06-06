using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(DeathManager))]
public class GameManager : MonoBehaviour
{
    #region Public Properties

    public static GameManager Instance { get { return _instance; } }

    public DeathManager DeathManager => _deathManager;

    #endregion

    #region Private Variables

    private static GameManager _instance;

    private DeathManager _deathManager;

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

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _deathManager = GetComponent<DeathManager>();
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
