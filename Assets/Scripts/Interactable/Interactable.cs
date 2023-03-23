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

    private InteractableHumble _interactableHumble;
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

    public InteractableHumble Humble => _interactableHumble;
    public Transform PlayerPositionTransform => _playerPositionTransform;
    public BaseInteractionController CurrentPlayer => _interactableHumble.CurrentPlayer;
    public float SingleUseTime => _singleUseTime;
    public InteractableHealth InteractableHealth => _interactableHealth;
    public Outline Outline => _outline;

    #endregion

    #region Unity Loops

    public virtual void Awake()
    {
        _interactableHealth = GetComponent<InteractableHealth>();
        _outline = GetComponent<Outline>();
        _outline.enabled = false;

        _interactableHumble = new InteractableHumble(this, _interactableHealth, _isAIOnlyInteractable);
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
        _interactableHumble.SetCurrentInteractable(other, isSetting, out bool setOutline);

        _outline.enabled = setOutline;
    }

    public void SetCurrentPlayer(BaseInteractionController interactionController)
    {
        _interactableHumble.SetCurrentPlayer(interactionController);
    }

    public void Uninteract()
    {
        _interactableHumble.Uninteract();
    }

    public void RemoveCurrentPlayer()
    {
        _interactableHumble.RemoveCurrentPlayer();
    }

    #endregion

    public bool IsBroken()
    {
        return _interactableHumble.IsBroken();
    }
}
