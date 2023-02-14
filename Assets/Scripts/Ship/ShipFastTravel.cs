using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ShipFastTravel : MonoBehaviour
{
    #region Getters and Setters

    public bool WantToTravel { get { return _wantToTravel; } set { _wantToTravel = value; } }

    public FastTravelNPC FastTravelNPC { get { return _fastTravelNPC; } set { _fastTravelNPC = value; } }

    #endregion

    #region Private Variables

    private int _playersInShip = 0;
    private int _playersActive = 0;

    private bool _wantToTravel = false;

    private ShipDoor _shipDoor;
    private PlayerInputHandler[] _isPlayerActive;
    private List<PlayerInputHandler> _playersInShipList = new List<PlayerInputHandler>();

    private FastTravelNPC _fastTravelNPC;

    private Ship _mainShip;

    private CameraManager _cameraManager;

    private Coroutine _lastRoutine = null;

    #endregion

    #region Unity Loops

    private void Start()
    {
        _mainShip = FindObjectOfType<Ship>();
        _isPlayerActive = FindObjectsOfType<PlayerInputHandler>();
        _shipDoor = _mainShip.GetComponentInChildren<ShipDoor>();
        _lastRoutine = StartCoroutine(DetachFromShip());
        _cameraManager = FindObjectOfType<CameraManager>();
        TimelinesManager.Instance.BlackHoleParticle.gameObject.transform.SetParent(_mainShip.transform);
        TimelinesManager.Instance.BlackHoleParticle.gameObject.transform.localPosition =new Vector3(2.5f,0,0);

        _mainShip.OnRespawn += HandleRespawn;
    }

    #endregion

    private void HandleRespawn()
    {
        _playersInShipList.Clear();
        _playersInShip = 0;
    }

    private void CheckPlayersInShip()
    {
        _playersActive = 0;
        for (int i = 0; i < _isPlayerActive.Length; i++)
        {
            if (_isPlayerActive[i].IsPlayerActive == true)
            {
                _playersActive++;
            }
        }

        if (_playersActive != _playersInShip) { return; }

        _cameraManager.ToggleCamera(true);

        if (_shipDoor.IsWantedDoorOpen == true) { return; }

        if (!_wantToTravel) { return; }

        _wantToTravel = false;
        _mainShip.gameObject.transform.SetParent(TimelinesManager.Instance.MainShipParentForTheTimeline.transform);
        StartCoroutine(FastTravelCoroutine());
        AttachToShip();
    }

    public void OnPlayerTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent<PlayerInputHandler>(out PlayerInputHandler player)) { return; }

        if (_playersInShipList.Contains(player)) { return; }

        _playersInShipList.Add(player);

        _playersInShip = Mathf.Clamp(_playersInShip+1, 0, _playersActive);

        if (_lastRoutine != null) { StopCoroutine(_lastRoutine); }
        AttachToShip();
        CheckPlayersInShip();
    }

    public void OnPlayerTriggerExit(Collider other)
    {
        if (!other.gameObject.TryGetComponent<PlayerInputHandler>(out PlayerInputHandler player)) { return; }

        if (_playersInShipList.Contains(player) == false) { return; }

        _cameraManager.ToggleCamera(false);
        _lastRoutine = StartCoroutine(DetachFromShip());
        _playersInShip = Mathf.Clamp(_playersInShip-1, 0, _playersActive);
        _playersInShipList.Remove(player);
    }

    private IEnumerator FastTravelCoroutine()
    {
        //Start the animation for the fast travel
        yield return new WaitForSeconds(4);
        TimelinesManager.Instance.StartFastTravelTimeline.Play();
        TimelinesManager.Instance.BlackHoleParticle.Play();
        //Stops the animation and takes the main ship out of its parent
        yield return new WaitForSeconds(2);
        TimelinesManager.Instance.StartFastTravelTimeline.Stop();
        TimelinesManager.Instance.EndFastTravelTimeline.Play();
        _mainShip.gameObject.transform.SetParent(null);
        _mainShip.gameObject.transform.position = _fastTravelNPC.TravelToPosition.transform.position;
        //Stops all remaining animations
        yield return new WaitForSeconds(2);
        TimelinesManager.Instance.EndFastTravelTimeline.Stop();
        TimelinesManager.Instance.BlackHoleParticle.Stop();
    }

    private void AttachToShip()
    {
        if(_isPlayerActive == null) { return; }

        for (int i = 0; i < _isPlayerActive.Length; i++)
        {
            if (_isPlayerActive[i].IsPlayerActive == true)
            {
               _isPlayerActive[i].GetComponentInParent<PlayerStateMachine>().AttachToShip(true);
            }
        }
    }

    private IEnumerator DetachFromShip()
    {
        yield return new WaitForSeconds(2);
        for (int i = 0; i < _isPlayerActive.Length; i++)
        {
            if (_isPlayerActive[i].IsPlayerActive == true)
            {
                _isPlayerActive[i].GetComponentInParent<PlayerStateMachine>().AttachToShip(false);
            }
        }
    }
}