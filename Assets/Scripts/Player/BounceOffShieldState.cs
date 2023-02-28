using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceOffShieldState : PlayerBaseState
{
    public BounceOffShieldState(BaseStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) 
    {
        _isRootState = true;
        InitializeSubStates();
    }

    private AIStateMachine _aiContext;
    private bool _isKinematicInitialState;
    private bool _useGravityInitialState;
    private bool _agentEnabledInitialState;

    public override void EnterState() 
    {
        _aiContext = _context as AIStateMachine;

        _isKinematicInitialState = _context.Rb.isKinematic;
        _useGravityInitialState = _context.Rb.useGravity;
        _agentEnabledInitialState = _aiContext.Agent.enabled;

        Debug.Log($"Enter Bounce State; {_aiContext.gameObject.name} {_aiContext.IsBouncingOffShield}");

        _context.Rb.isKinematic = false;
        _context.Rb.useGravity = true;
        _aiContext.Agent.enabled = false;
        _context.GetComponent<Collider>().isTrigger = false; 
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState() 
    {
        Debug.Log($"Exit Bounce State; {_aiContext.gameObject.name} {_aiContext.IsBouncingOffShield}");
        _context.Rb.isKinematic = _isKinematicInitialState;
        _context.Rb.useGravity = _useGravityInitialState;
        _aiContext.Agent.enabled = _agentEnabledInitialState;
        _context.GetComponent<Collider>().isTrigger = true;
    }

    public override void InitializeSubStates() { }

    public override void CheckSwitchStates()
    {
        if(_aiContext == null) { SwitchState(_factory.Grounded()); }

        if (_context.PlayerHealth.IsHurt || _context.PlayerHealth.IsDead())
        {
            SwitchState(_factory.Ragdoll());
        }
        else if (_aiContext.IsBouncingOffShield == false)
        {
            SwitchState(_factory.Grounded());
        }
    }
}
