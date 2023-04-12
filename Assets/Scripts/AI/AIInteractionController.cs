//#define INTERACTION_DEBUGS

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

        if(CurrentInteractable.AttachOnInteract == false) { return; }

        transform.position = CurrentInteractable.PlayerPositionTransform.position;
        transform.rotation = CurrentInteractable.PlayerPositionTransform.rotation;
    }

    private void OnDestroy()
    {
        _gAgent.OnExitAction -= CheckExitInteraction;
    }

    public override void SetCurrentInteractable(Interactable interactable)
    {
#if INTERACTION_DEBUGS
        Debug.Log("1-SetCurrentInteractable: " + gameObject.name);
#endif
        if (!_canInteractInCurrentState) { return; }

#if INTERACTION_DEBUGS
        Debug.Log("2-_canInteractInCurrentState: " + gameObject.name);
#endif

        if (interactable == null) { return; }

#if INTERACTION_DEBUGS
        Debug.Log("3-interactable != null: " + gameObject.name);
#endif

        if(_gAgent.CurrentAction == null || _gAgent.CurrentAction.Target == null || _gAgent.CurrentAction.IsRunning == false) { return; }

#if INTERACTION_DEBUGS
        Debug.Log("4-has action!: " + gameObject.name);
#endif
        //Only interact with intended interactable
        if(_gAgent.CurrentAction.Target.transform != interactable.transform) { return; }

#if INTERACTION_DEBUGS
        Debug.Log($"4.5-_gAgent.CurrentAction.Target.transform: {_gAgent.CurrentAction.Target.transform} !=  interactable.transform: {interactable.transform} " + gameObject.name);
#endif

        base.SetCurrentInteractable(interactable);

        Interactable wantedInteractable = _gAgent?.CurrentAction?.Target?.GetComponent<Interactable>();

        if(interactable == wantedInteractable)
        {
#if INTERACTION_DEBUGS
            Debug.Log("5-is wanted Interaction!: " + gameObject.name);
#endif
            HandleInteraction(_gAgent.CurrentAction.Duration);
        }
    }

    public override void HandleChangeState(PlayerBaseState newState, bool isRootState)
    {
        if (!isRootState) { return; }

        if (newState is PlayerGroundedState) { _canInteractInCurrentState = true; return; }

        _canInteractInCurrentState = false;

        CheckExitInteraction();
    }

    public override void CheckExitInteraction()
    {
        base.CheckExitInteraction();
    }
}
