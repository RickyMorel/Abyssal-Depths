using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReselectButton : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject _reselectButton;
    [Tooltip("Leave it as null for main menu and things like that")]
    [SerializeField] private Interactable _interactable = null;

    #endregion

    #region Private Variable

    private EventSystem _eventSystem;
    private PlayerInputHandler _playerInput;

    #endregion

    #region Unity Loops

    private void Start()
    {
        _eventSystem = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<EventSystem>();

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
        _playerInput.OnUIHorizontal += ReselectButtonWhenNeeded;
        _playerInput.OnUIVertical += ReselectButtonWhenNeeded;
        _playerInput.OnShoulderLeft += ReselectButtonWhenNeeded;
        _playerInput.OnShoulderRight += ReselectButtonWhenNeeded;
        _playerInput.OnUISubmit += ReselectButtonWhenNeeded;
        _playerInput.OnUICancel += ReselectButtonWhenNeeded;
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

    private void ReselectButtonWhenNeeded()
    {
        Debug.Log(_eventSystem.currentSelectedGameObject);
    }
}