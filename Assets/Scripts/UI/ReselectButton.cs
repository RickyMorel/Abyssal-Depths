using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Rewired.Integration.UnityUI;
using UnityEngine.UI;

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
        _eventSystem = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<RewiredEventSystem>();

        _playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInputHandler>();

        if (_firstButton == null) { _firstButton = GetComponentInChildren<Button>().gameObject; }
    }

    private void OnEnable()
    {
        StartCoroutine(LateEnable());
    }

    private void OnDisable()
    {
        _playerInput.OnUIHorizontal -= ReselectButtonWhenNeeded;
        _playerInput.OnUIVertical -= ReselectButtonWhenNeeded;
        _playerInput.OnShoulderLeft -= ReselectButtonWhenNeeded;
        _playerInput.OnShoulderRight -= ReselectButtonWhenNeeded;
        _playerInput.OnUISubmit -= ReselectButtonWhenNeeded;

        _playerInput.OnUICancel -= GoBackToPreviousScreenOrExit;
    }

    #endregion

    private IEnumerator LateEnable()
    {
        yield return new WaitForEndOfFrame();

        _playerInput.OnUIHorizontal += ReselectButtonWhenNeeded;
        _playerInput.OnUIVertical += ReselectButtonWhenNeeded;
        _playerInput.OnShoulderLeft += ReselectButtonWhenNeeded;
        _playerInput.OnShoulderRight += ReselectButtonWhenNeeded;
        _playerInput.OnUISubmit += ReselectButtonWhenNeeded;

        _playerInput.OnUICancel += GoBackToPreviousScreenOrExit;
    }

    private void ReselectButtonWhenNeeded()
    {
        if (_eventSystem.currentSelectedGameObject != null && _eventSystem.currentSelectedGameObject.activeInHierarchy) 
        {
            _lastSelected = _eventSystem.currentSelectedGameObject;
        }
        else if (_lastSelected != null)
        {
            _eventSystem.RewiredCurrentSelectedGameObject = _lastSelected; 
        }
        else
        {
            _eventSystem.RewiredCurrentSelectedGameObject = _firstButton;
        }
    }

    private void GoBackToPreviousScreenOrExit()
    {
        if (_previousScreen.Length == 0) { return; }

        for (int i = 0; i < _previousScreen.Length; i++)
        {
            _previousScreen[i].SetActive(true);
            gameObject.SetActive(false);
        }
    }
}