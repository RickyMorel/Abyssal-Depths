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
    [SerializeField] private bool _attachOnInteract = true;

    [Header("One Time Interaction Parameters")]
    [SerializeField] private bool _isSingleUse = false;
    [SerializeField] private float _singleUseTime = 0.5f;

    [Header("FX")]
    [SerializeField] private ParticleSystem _onInteractParticles;

    #endregion

    #region Private Variables

    protected InteractableHumble _humble;
    private InteractableHealth _interactableHealth;
    private Outline _outline;
    private bool _canUse = true;

    #endregion

    #region Getters && Setters

    public InteractionType InteractionType { get { return _interactionType; } set { _interactionType = value; } }
    public bool IsSingleUse { get { return _isSingleUse; } set { _isSingleUse = value; } }
    public bool CanUse { get { return _canUse; } set { _canUse = value; } }

    #endregion

    #region Public Properties

    public InteractableHumble Humble => _humble;
    public Transform PlayerPositionTransform => _playerPositionTransform;
    public float SingleUseTime => _singleUseTime;
    public InteractableHealth InteractableHealth => _interactableHealth;
    public Outline Outline => _outline;
    public bool IsAIOnlyInteractable => _isAIOnlyInteractable;
    public bool AttachOnInteract => _attachOnInteract;
    public BaseInteractionController CurrentPlayer => _humble.CurrentPlayer;

    #endregion

    #region Unity Loops

    public virtual void Awake()
    {
        _interactableHealth = GetComponent<InteractableHealth>();
        _outline = GetComponent<Outline>();
        _outline.enabled = false;

        _humble = new InteractableHumble(_isAIOnlyInteractable);
    }

    //Ensures that CurrentPlayer gets set to null when player is done interacting
    private void LateUpdate()
    {
        if(CurrentPlayer == null) { return; }

        if(CurrentPlayer.CurrentInteraction != 0) { return; }

        _humble.SetCurrentPlayer(null);
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        SetCurrentInteractable(other, true);
    }

    public virtual void OnTriggerExit(Collider other)
    {
        SetCurrentInteractable(other, false);
    }

    private void SetCurrentInteractable(Collider other, bool isSetting)
    {
        _humble.SetCurrentInteractable(other, isSetting, out BaseInteractionController interactionController, out bool setInteractable, out bool setOutline);

        interactionController?.SetCurrentInteractable(setInteractable ? this : null);

        _outline.enabled = setOutline;
    }

    public void SetCurrentPlayer(BaseInteractionController interactionController)
    {
        _humble.SetCurrentPlayer(interactionController);

        if(_onInteractParticles != null) { _onInteractParticles.Play(); }
    }

    public void Uninteract()
    {
        _humble.Uninteract();
    }

    public void RemoveCurrentPlayer()
    {
        _humble.RemoveCurrentPlayer();
    }

    #endregion

    public bool IsBroken()
    {
        if (_interactableHealth == null) { return false; }

        return _interactableHealth.IsDead();
    }
}
