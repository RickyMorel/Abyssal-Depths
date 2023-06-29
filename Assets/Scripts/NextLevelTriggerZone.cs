using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class NextLevelTriggerZone : MonoBehaviour
{
    #region Editor Fields

    [Header("Timeline")]
    [SerializeField] private PlayableDirector _playableDirector;
    [SerializeField] private TimelineAsset _entranceTimeline;
    [SerializeField] private TimelineAsset _openTimeline;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _portalParticle;
    [SerializeField] private ParticleSystem _rockDustParticle;

    [Header("Interaction")]
    [SerializeField] private Transform _transformForImmobileShip;
    [SerializeField] private float _immobileShipMoveSpeed = 10;
    [SerializeField] private Canvas _portalKeyInteractionCanvas;

    #endregion

    #region Private Variables

    private bool _updateCheck = false;
    private bool _isInteracting = false;
    private PlayerInputHandler _playerInput;
    private PlayerCarryController _playerCarryController;
    private bool _isInPhase2 = false;
    private bool _isInPhase3 = false;

    #endregion

    #region Public Properties

    public static event Action<int> OnLevelCompleted;
    public bool IsInPhase3 => _isInPhase3;

    #endregion

    #region Unity Loops

    private void Start()
    {
        _portalKeyInteractionCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!_updateCheck) { return; }

        if (Ship.Instance.ShipLandingController.Booster.CurrentPlayer != null) { Ship.Instance.ShipLandingController.Booster.CurrentPlayer.CheckExitInteractionWhenNotRepairing(); }

        if (!_isInPhase2 && !_isInPhase3) { return; }

        Ship.Instance.AddForceToShip((_transformForImmobileShip.position - Ship.Instance.transform.position).normalized * _immobileShipMoveSpeed, ForceMode.Force);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Ship _)) { return; }

        if (Ship.Instance.ShipLandingController.Booster.CurrentPlayer == null) { return; }

        if (_playerInput == null) { _playerInput = Ship.Instance.ShipLandingController.Booster.CurrentPlayer.GetComponent<PlayerInputHandler>(); }

        if (_playerCarryController == null) { _playerCarryController = _playerInput.GetComponent<PlayerCarryController>(); }

        if (_playerCarryController.CurrentSingleObjInstance == null) { _portalKeyInteractionCanvas.gameObject.SetActive(false); return; }

        if (_playerCarryController.CurrentSingleObjInstance.tag != GameTagsManager.PORTAL_KEY) { _portalKeyInteractionCanvas.gameObject.SetActive(false); return; }

        _portalKeyInteractionCanvas.gameObject.SetActive(true);

        if (!_playerInput.IsShooting_2) { return; }
        
        if (_isInteracting) { return; }

        _isInteracting = true;

        PortalPhase1();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Ship _)) { return; }

        _isInteracting = false;

        _portalKeyInteractionCanvas.gameObject.SetActive(false);
    }

    #endregion

    private void PortalPhase1()
    {
        if (!_isInteracting) { return; }

        _portalKeyInteractionCanvas.gameObject.SetActive(false);
        Destroy(_playerCarryController.CurrentSingleObjInstance);
        Ship.Instance.ShipLandingController.Booster.LockHovering = true;
        Ship.Instance.ShipLandingController.Booster.SetIsHovering(true);
        _updateCheck = true;

        PortalPhase2();
    }

    private void PortalPhase2()
    {
        //todo: minigame or something idk
        
        _isInPhase2 = true;

        _isInPhase2 = false;

        StartCoroutine(PortalPhase3());
    }

    private IEnumerator PortalPhase3()
    {
        _playableDirector.playableAsset = _entranceTimeline;
        _playableDirector.Play();

        ShipCamera.Instance.ShakeCamera(2, 5, 30);

        _isInPhase3 = true;
        _rockDustParticle.Play();

        yield return new WaitForSeconds(30f);

        _rockDustParticle.Stop();
        _isInPhase3 = false;

        PortalPhase4();
    }

    public void PortalPhase4()
    {
        _playableDirector.playableAsset = _openTimeline;
        _playableDirector.Play();

        _portalParticle.Play();
        _updateCheck = false;

        OnLevelCompleted?.Invoke(SceneManager.GetActiveScene().buildIndex);
    }
    //1 esta cerrado en los estados siguientes la nave no se debe mover hasta el final, quedandose en el lugar flotando
    //2 el jugador interactua con la puerta y esta vibra
    //3 la puerta empieza a hacer algo y vienen enemigos por 30 segundos
    //4 la puerta se abre
}