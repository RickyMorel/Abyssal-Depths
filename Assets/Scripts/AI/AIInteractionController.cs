using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInteractionController : BaseInteractionController
{
    #region Private Variables

    private GAgent _gAgent;

    #endregion

    public override void Start()
    {
        base.Start();

        _gAgent = GetComponent<GAgent>();

        _gAgent.OnExitAction += CheckExitInteraction;
    }

    private void FixedUpdate()
    {
        if(CurrentInteractable == null) { return; }

        if(_currentInteraction == 0) { return; }

        transform.position = CurrentInteractable.PlayerPositionTransform.position;
        transform.rotation = CurrentInteractable.PlayerPositionTransform.rotation;
    }

    private void OnDestroy()
    {
        _gAgent.OnExitAction -= CheckExitInteraction;
    }

    public override void SetCurrentInteractable(Interactable interactable)
    {
        base.SetCurrentInteractable(interactable);

        if(interactable == null) { return; }

        if(_gAgent.CurrentAction == null || _gAgent.CurrentAction.Target == null || _gAgent.CurrentAction.IsRunning == false) { return; }

        Interactable wantedInteractable = _gAgent?.CurrentAction?.Target?.GetComponent<Interactable>();

        if(interactable == wantedInteractable)
        {
            HandleInteraction(_gAgent.CurrentAction.Duration);
        }
    }

    public override void CheckExitInteraction()
    {
        base.CheckExitInteraction();
    }
}
