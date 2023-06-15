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

    public override void EnterState() 
    {
        _aiContext = _context as AIStateMachine;

        _context.PlayerRagdoll.EnableLivingRagdoll(true);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState() 
    {
        _context.PlayerRagdoll.EnableLivingRagdoll(false);
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
