using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(BaseStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void EnterState() 
    {
        _context.Speed = 6;
    }

    public override void UpdateState() 
    {
        _context.Speed = 6;

        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void InitializeSubStates() { }

    public override void CheckSwitchStates()
    {
        if (_context.IsShooting)
        {
            SwitchState(_factory.Attack());
        }
        else if (_context.PlayerCarryController != null && _context.PlayerCarryController.HasItems)
        {
            SwitchState(_factory.Carry());
        }
        else if(_context.MoveDirection.magnitude > 0f)
        {
            SwitchState(_factory.Run());
        }
    }
}
