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

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _deathManager = GetComponent<DeathManager>();
        _levelData = GetComponent<LevelData>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void GameOver()
    {
        StartCoroutine(GameOverCoroutine());
    }

    private IEnumerator GameOverCoroutine()
    {
        yield return new WaitForSeconds(1f);
    }
}
