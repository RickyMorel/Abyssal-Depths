using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Rewired.Integration.UnityUI;
using UnityEngine.UI;
using Rewired;

public class ReselectButton : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] protected GameObject _firstButton;
    [Tooltip("Leave it as null if it can't go back to a previous screen any longer")]
    [SerializeField] protected GameObject[] _previousScreen = null;

    #endregion

    #region Private Variable

    protected RewiredEventSystem _eventSystem;
    protected PlayerInputHandler _playerInput;
    protected GameObject _lastSelected;

    #endregion

    #region Unity Loops

    public virtual void Start()
    {
        _eventSystem = GameObject.FindGameObjectWithTag(GameTagsManager.EVENT_SYSTEM).GetComponent<RewiredEventSystem>();

        _playerInput = GameObject.FindGameObjectWithTag(GameTagsManager.PLAYER).GetComponent<PlayerInputHandler>();

        if (_firstButton == null) { _firstButton = GetComponentInChildren<Button>().gameObject; }
    }

    private void OnEnable()
    {
        StartCoroutine(LateEnable());
    }

    public virtual void OnDisable()
    {
        _playerInput.OnUIHorizontal -= ReselectButtonWhenNeeded;
        _playerInput.OnUIVertical -= ReselectButtonWhenNeeded;
        _playerInput.OnUISubmit -= ReselectButtonWhenNeeded;

        _playerInput.OnUICancel -= GoBackToPreviousScreenOrExit;
    }

    #endregion

    public virtual IEnumerator LateEnable()
    {
        yield return new WaitForEndOfFrame();

        Player rewiredPlayer = ReInput.players.GetPlayer(_playerInput.PlayerId);
        rewiredPlayer.controllers.maps.SetMapsEnabled(true, "UI");

        _playerInput.OnUIHorizontal += ReselectButtonWhenNeeded;
        _playerInput.OnUIVertical += ReselectButtonWhenNeeded;
        _playerInput.OnUISubmit += ReselectButtonWhenNeeded;

        _playerInput.OnUICancel += GoBackToPreviousScreenOrExit;
    }

    public void ReselectButtonWhenNeeded()
    {
        if (_eventSystem.currentSelectedGameObject != null && _eventSystem.currentSelectedGameObject.activeInHierarchy) 
        {
            _lastSelected = _eventSystem.currentSelectedGameObject;
        }
        else if (_lastSelected != null)
        {
            _eventSystem.SetSelectedGameObject(_lastSelected);
        }
        else
        {
            _eventSystem.SetSelectedGameObject(_firstButton);
        }
    }

    public void GoBackToPreviousScreenOrExit()
    {
        if (_previousScreen.Length == 0) { return; }

        for (int i = 0; i < _previousScreen.Length; i++)
        {
            _previousScreen[i].SetActive(true);
            gameObject.SetActive(false);
        }
    }
}