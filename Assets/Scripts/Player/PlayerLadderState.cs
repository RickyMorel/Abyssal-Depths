using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLadderState : PlayerBaseState
{
    public PlayerLadderState(BaseStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) 
    {
        _isRootState = true;
        InitializeSubStates();
    }

    private AIStateMachine _aiContext;

    public override void EnterState()
    {
        _aiContext = _context as AIStateMachine;
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void InitializeSubStates() 
    {
        if(_context.MoveDirection.magnitude == 0f)
        {
            SetSubState(_factory.Idle());
        }
        else
        {
            SetSubState(_factory.Run());
        }
    }

    public override void CheckSwitchStates()
    {
        if (_context.PlayerHealth.IsHurt || _context.PlayerHealth.IsDead())
        {
            SwitchState(_factory.Ragdoll());
        }
        if(_aiContext != null && _aiContext.IsBouncingOffShield)
        {
            SwitchState(_factory.BounceOff());
        }
        else if (_context.IsJumpPressed)
        {
            SwitchState(_factory.Jump());
        }
        else if (!_context.IsGrounded)
        {
            SwitchState(_factory.Fall());
        }
    }
}
