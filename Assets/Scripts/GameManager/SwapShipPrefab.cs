using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapShipPrefab : MonoBehaviour
{
    #region Private Variables

    private GameObject _currentShip;
    private PlayerInputHandler[] _playersActive;

    #endregion

    #region Editor Fields

    [Header("GameObjects")]
    [SerializeField] private GameObject _4playersShipPrefab;
    [SerializeField] private GameObject _3playersShipPrefab;
    [SerializeField] private GameObject _2playersShipPrefab;
    [Header("Transform")]
    [SerializeField] private Transform _4playersShipTransform;
    [SerializeField] private Transform _3playersShipTransform;
    [SerializeField] private Transform _2playersShipTransform;

    #endregion

    #region Unity Loops

    private void Start()
    {
        _playersActive = FindObjectsOfType<PlayerInputHandler>();
    }

    private void Awake()
    {
        PlayerJoinManager.OnPlayerJoin += HandleNewPlayer;
    }

    private void OnDestroy()
    {
        PlayerJoinManager.OnPlayerJoin -= HandleNewPlayer;
    }

    #endregion

    private void HandleNewPlayer()
    {
        _playersActive = FindObjectsOfType<PlayerInputHandler>();

        SwapShips(_playersActive.Length);
    }

    private void SwapShips(int playerAmount)
    {
        switch (playerAmount)
        {
            case 2:
                _currentShip = Instantiate(_2playersShipPrefab);
                _currentShip.GetComponent<ShipMovingStaticManager>().SetShipState(true, false);
                _currentShip.transform.position = _2playersShipTransform.position;
                break;
            case 3:
                Destroy(_currentShip);
                _currentShip = Instantiate(_2playersShipPrefab);
                _currentShip.transform.position = _2playersShipTransform.position;
                break;
            case 4:
                Destroy(_currentShip);
                _currentShip = Instantiate(_2playersShipPrefab);
                _currentShip.transform.position = _2playersShipTransform.position;
                break;
        }
    }
}