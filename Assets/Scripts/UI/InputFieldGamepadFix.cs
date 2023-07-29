using Rewired;
using Rewired.Integration.UnityUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputFieldGamepadFix : MonoBehaviour
{
    #region Private Variables

    private RewiredEventSystem _eventSystem;
    private PlayerInputHandler _playerInput;
    private float _timerForSelectedGameObject;

    #endregion

    #region Editor Fields

    [SerializeField] private List<GameObject> _gameObjects;

    #endregion

    #region Unity Loops

    private void Start()
    {
        _eventSystem = GameObject.FindGameObjectWithTag(GameTagsManager.EVENT_SYSTEM).GetComponent<RewiredEventSystem>();
        _playerInput = GameObject.FindGameObjectWithTag(GameTagsManager.PLAYER).GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        if (_eventSystem.currentSelectedGameObject == gameObject)
        {
            _timerForSelectedGameObject = _timerForSelectedGameObject + Time.deltaTime;
        }
        else
        {
            _timerForSelectedGameObject = 0;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(LateEnable());
    }

    private void OnDisable()
    {
        _playerInput.OnUIHorizontal -= CheckIfInputFieldIsSelected;
        _playerInput.OnUIVertical -= CheckIfInputFieldIsSelected;
        _playerInput.OnUISubmit -= CheckIfInputFieldIsSelected;
    }

    #endregion

    private IEnumerator LateEnable()
    {
        yield return new WaitForEndOfFrame();

        Player rewiredPlayer = ReInput.players.GetPlayer(_playerInput.PlayerId);
        rewiredPlayer.controllers.maps.SetMapsEnabled(true, "UI");

        _playerInput.OnUIHorizontal += CheckIfInputFieldIsSelected;
        _playerInput.OnUIVertical += CheckIfInputFieldIsSelected;
        _playerInput.OnUISubmit += CheckIfInputFieldIsSelected;
    }

    private void CheckIfInputFieldIsSelected()
    {
        if (_eventSystem.currentSelectedGameObject == gameObject)
        {
            SelectOtherGameObject();
        }
    }

    private void SelectOtherGameObject()
    {
        if (_timerForSelectedGameObject < 0.5f) { return; }
        
        if (_playerInput.PlayerInputs.GetAxis("UIVertical") < 0)
        {
            _eventSystem.SetSelectedGameObject(_gameObjects[0]);
        }
        if (_playerInput.PlayerInputs.GetAxis("UIVertical") > 0)
        {
            _eventSystem.SetSelectedGameObject(_gameObjects[1]);
        }
        if (_playerInput.PlayerInputs.GetAxis("UIHorizontal") < 0)
        {
            _eventSystem.SetSelectedGameObject(_gameObjects[2]);
        }
        if (_playerInput.PlayerInputs.GetAxis("UIHorizontal") > 0)
        {
            _eventSystem.SetSelectedGameObject(_gameObjects[3]);
        }
        if (_playerInput.PlayerInputs.GetButton("UISubmit"))
        {
            _eventSystem.SetSelectedGameObject(_gameObjects[4]);
            _gameObjects[5].SetActive(true);
        }
    }
}