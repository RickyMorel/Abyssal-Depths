using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Rewired.Integration.UnityUI;

public class ReselectButton : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject _firstButton;
    [Tooltip("Leave it as null for main menu and things like that")]
    [SerializeField] private Interactable _interactable = null;

    #endregion

    #region Private Variable

    private RewiredEventSystem _eventSystem;
    private PlayerInputHandler _playerInput;
    private GameObject _lastSelected;

    #endregion

    #region Unity Loops

    private void Start()
    {
        _eventSystem = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<RewiredEventSystem>();

        if (_interactable == null)
        {
            _playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInputHandler>();
        }
        else
        {
            _playerInput = _interactable.CurrentPlayer.GetComponent<PlayerInputHandler>();
        }
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
        _playerInput.OnUICancel -= ReselectButtonWhenNeeded;
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
        _playerInput.OnUICancel += ReselectButtonWhenNeeded;
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
}