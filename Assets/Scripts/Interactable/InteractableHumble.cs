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

    private bool _isAIOnlyInteractable;

    #endregion

    #region Public Properties

    public BaseInteractionController CurrentPlayer => _currentPlayer;

    public event Action OnInteract;
    public event Action OnUninteract;
    public event Action OnSetInteractable;

    #endregion

    public InteractableHumble(bool isAIOnlyInteractable)
    {
        _isAIOnlyInteractable = isAIOnlyInteractable;
    }

    public bool SetCurrentInteractable(Collider other, bool isSetting, out BaseInteractionController interactionController, out bool setInteractable, out bool setOutline)
    {
        setOutline = false;
        setInteractable = false;
        interactionController = null;

        if (!other.gameObject.TryGetComponent(out BaseInteractionController InteractionController)) { return false; }

        interactionController = InteractionController;

        if (interactionController is PlayerInteractionController)
        {
            if (_isAIOnlyInteractable) { return false; }

            setOutline = isSetting;

            if (!isSetting)
            {
                //Only sets current interactable null for players so they don't teleport to it when pressing the interact button
                setInteractable = false;
                OnSetInteractable?.Invoke();
            }
        }

        if (isSetting) { setInteractable = true; OnSetInteractable?.Invoke(); }

        return true;
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
}
