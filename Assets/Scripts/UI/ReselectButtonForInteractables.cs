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
        _eventSystem = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<RewiredEventSystem>();

        _playerInput = _interactable.CurrentPlayer.GetComponent<PlayerInputHandler>();

        if (_firstButton == null) { _firstButton = GetComponentInChildren<Button>().gameObject; }
    }

    #endregion
}