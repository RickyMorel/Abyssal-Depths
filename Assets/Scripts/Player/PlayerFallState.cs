using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState(BaseStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) 
    {
        _isRootState = true;
    }

    public override void EnterState() { }

    public override void UpdateState()
    {
        if (!_context.IsGrounded) { SetIsFalling(true); }

        CheckSwitchStates();
    }

    public override void ExitState()
    {
        SetIsFalling(false);
    }

    public override void InitializeSubStates() { }

    public override void CheckSwitchStates()
    {
        if (_context.PlayerHealth.IsHurt || _context.PlayerHealth.IsDead())
        {
            SwitchState(_factory.Ragdoll());
        }
        else if (_context.IsGrounded)
        {
            SwitchState(_factory.Grounded());
        }
    }

    private void SetIsFalling(bool isFalling)
    {
        _context.Anim.SetBool("isFalling", isFalling);

        //Don't play jump anim while in air 
        if (isFalling) { _context.Anim.ResetTrigger("Jump"); }
    }
}
