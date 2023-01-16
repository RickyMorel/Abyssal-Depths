using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Interactable : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private InteractionType _interactionType;
    [SerializeField] private Transform _playerPositionTransform;
    [SerializeField] private bool _isAIOnlyInteractable = false;

    [Header("One Time Interaction Parameters")]
    [SerializeField] private bool _isSingleUse = false;
    [SerializeField] private float _singleUseTime = 0.5f;

    #endregion

    #region Private Variables

    private InteractableHealth _interactableHealth;
    private Outline _outline;
    private bool _canUse = true;

    #endregion

    #region Protected Variables

    protected BaseInteractionController _currentPlayer;

    #endregion

    #region Getters && Setters

    public InteractionType InteractionType { get { return _interactionType; } set { _interactionType = value; } }
    public bool IsSingleUse { get { return _isSingleUse; } set { _isSingleUse = value; } }
    public bool CanUse { get { return _canUse; } set { _canUse = value; } }

    #endregion

    #region Public Properties

    public Transform PlayerPositionTransform => _playerPositionTransform;
    public BaseInteractionController CurrentPlayer => _currentPlayer;
    public float SingleUseTime => _singleUseTime;
    public InteractableHealth InteractableHealth => _interactableHealth;
    public Outline Outline => _outline;

    public event Action OnInteract;
    public event Action OnUninteract;

    #endregion

    #region Unity Loops

    public virtual void Awake()
    {
        _interactableHealth = GetComponent<InteractableHealth>();
        _outline = GetComponent<Outline>();

        _outline.enabled = false;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent<BaseInteractionController>(out BaseInteractionController interactionController)) { return; }

        if (interactionController is PlayerInteractionController) 
        {
            if (_isAIOnlyInteractable) { return; }

            _outline.enabled = true; 
        }

        interactionController.SetCurrentInteractable(this);
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.TryGetComponent<BaseInteractionController>(out BaseInteractionController interactionController)) { return; }

        if (interactionController is PlayerInteractionController) 
        {
            if (_isAIOnlyInteractable) { return; }

            _outline.enabled = false;

            //Only sets current interactable null for players so they don't teleport to it when pressing the interact button
            interactionController.SetCurrentInteractable(null);
        }
    }

    public void SetCurrentPlayer(BaseInteractionController interactionController)
    {
        _currentPlayer = interactionController;

        OnInteract?.Invoke();
    }

    public void Uninteract()
    {
        OnUninteract?.Invoke();
    }

    public void RemoveCurrentPlayer()
    {
        if(_currentPlayer == null) { return; }

        _currentPlayer.CheckExitInteraction();
    }

    #endregion

    public bool IsBroken()
    {
        if(_interactableHealth == null) { return false; }

        return _interactableHealth.IsDead();
    }
}
