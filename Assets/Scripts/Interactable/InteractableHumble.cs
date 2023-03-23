using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InteractableHumble
{
    #region Protected Variables

    protected BaseInteractionController _currentPlayer;

    #endregion

    #region Private Variables

    private Interactable _interactableMono;
    private InteractableHealth _interactableHealth;
    private bool _isAIOnlyInteractable;

    #endregion

    #region Public Properties

    public BaseInteractionController CurrentPlayer => _currentPlayer;

    public event Action OnInteract;
    public event Action OnUninteract;
    public event Action OnSetInteractable;

    #endregion

    public InteractableHumble(Interactable interactable, InteractableHealth interactableHealth, bool isAIOnlyInteractable)
    {
        _interactableMono = interactable;
        _interactableHealth = interactableHealth;
        _isAIOnlyInteractable = isAIOnlyInteractable;
    }

    public void SetCurrentInteractable(Collider other, bool isSetting, out bool setOutline)
    {
        setOutline = false;

        if (!other.gameObject.TryGetComponent(out BaseInteractionController interactionController)) { return; }

        if (interactionController is PlayerInteractionController)
        {
            if (_isAIOnlyInteractable) { return; }

            setOutline = isSetting;

            if (!isSetting)
            {
                //Only sets current interactable null for players so they don't teleport to it when pressing the interact button
                interactionController.SetCurrentInteractable(isSetting ? _interactableMono : null);
                OnSetInteractable?.Invoke();
            }
        }

        if (isSetting) { interactionController.SetCurrentInteractable(isSetting ? _interactableMono : null); OnSetInteractable?.Invoke(); }
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

    public bool RemoveCurrentPlayer()
    {
        if (_currentPlayer == null) { return false; }

        _currentPlayer.CheckExitInteraction();

        return true;
    }

    public bool IsBroken()
    {
        if (_interactableHealth == null) { return false; }

        return _interactableHealth.IsDead();
    }
}
