using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    private void Start()
    {
        _deathManager = GetComponent<DeathManager>();
    }
}
