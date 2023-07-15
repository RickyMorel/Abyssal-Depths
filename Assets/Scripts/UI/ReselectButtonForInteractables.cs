using Rewired;
using Rewired.Integration.UnityUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReselectButtonForInteractables : ReselectButton
{
    #region Editor Fields

    [SerializeField] private Interactable _interactable = null;

    #endregion

    #region Public Properties

    public Interactable Interactable { get { return _interactable; } set { _interactable = value; } }
    public GameObject LastSelected => _lastSelected;
    public RewiredEventSystem RewiredEventSystem => _eventSystem;

    #endregion

    #region Unity Loops

    public override void Start()
    {
        //do nothing
    }
    public override void OnDisable()
    {
        //do nothing
    }

    #endregion

    public override IEnumerator LateEnable()
    {
        yield return new WaitForEndOfFrame();

        if (_firstButton == null) { _firstButton = GetComponentInChildren<Button>().gameObject; }

        Player rewiredPlayer = ReInput.players.GetPlayer(_interactable.CurrentPlayer.PlayerInput.PlayerId);
        rewiredPlayer.controllers.maps.SetMapsEnabled(true, "UI");

        _playerInput = _interactable.CurrentPlayer.PlayerInput;
        _playerInput.OnUIHorizontal += ReselectButtonWhenNeeded;
        _playerInput.OnUIVertical += ReselectButtonWhenNeeded;
        _playerInput.OnShoulderLeft += ReselectButtonWhenNeeded;
        _playerInput.OnShoulderRight += ReselectButtonWhenNeeded;
        _playerInput.OnUISubmit += ReselectButtonWhenNeeded;

        _playerInput.OnUICancel += GoBackToPreviousScreenOrExit;

        _interactable.Humble.OnUninteract += Unsuscribe;

        _eventSystem = GameObject.FindGameObjectWithTag(GameTagsManager.EVENT_SYSTEM).GetComponent<RewiredEventSystem>();
    }

    private void Unsuscribe()
    {
        _playerInput.OnUIHorizontal -= ReselectButtonWhenNeeded;
        _playerInput.OnUIVertical -= ReselectButtonWhenNeeded;
        _playerInput.OnShoulderLeft -= ReselectButtonWhenNeeded;
        _playerInput.OnShoulderRight -= ReselectButtonWhenNeeded;
        _playerInput.OnUISubmit -= ReselectButtonWhenNeeded;

        _playerInput.OnUICancel -= GoBackToPreviousScreenOrExit;

        _interactable.Humble.OnUninteract -= Unsuscribe;
    }
}