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
        SwapShips(_playersActive.Length);
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
        if (ShipMovingStaticManager.Instance != null) { Destroy(ShipMovingStaticManager.Instance.gameObject); }
        switch (playerAmount)
        {
            case 1 or 2:
                StartCoroutine(InstantiateShip1FrameAfter(_2playersShipPrefab, _2playersShipTransform));
                break;
            case 3:
                StartCoroutine(InstantiateShip1FrameAfter(_3playersShipPrefab, _3playersShipTransform));
                break;
            case 4:
                StartCoroutine(InstantiateShip1FrameAfter(_4playersShipPrefab, _4playersShipTransform));
                break;
        }
        CameraManager.Instance.SplitScreen(_playersActive.Length-2);
    }

    private IEnumerator InstantiateShip1FrameAfter(GameObject shipPrefab, Transform spawnTransform)
    {
        yield return new WaitForEndOfFrame();
        _currentShip = Instantiate(shipPrefab, spawnTransform.position, Quaternion.identity);
        yield return new WaitForEndOfFrame();
        _currentShip.GetComponent<ShipMovingStaticManager>().ShouldTeleport = false;
    }
}