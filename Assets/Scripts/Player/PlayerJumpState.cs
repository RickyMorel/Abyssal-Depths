using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {
        _isRootState = true;
    }

    public override void EnterState()
    {
        HandleJump();
    }

    public override void UpdateState() 
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {

    }

    public override void InitializeSubStates() { }

    public override void CheckSwitchStates() 
    {
        if (_context.IsGrounded)
        {
            SwitchState(_factory.Grounded());
        }
        else
        {
            SwitchState(_factory.Fall());
        }
    }

    private void HandleJump()
    {
        if (_context.PlayerInteraction.HasRecentlyInteracted()) { return; }

        _context.Anim.SetTrigger("Jump");

        float jumpingVelocity = Mathf.Sqrt(-2 * _context.GravityIntensity * _context.JumpHeight);
        Vector3 playerVelocity = _context.MoveDirection;
        playerVelocity.y = jumpingVelocity;
        _context.Rb.velocity = playerVelocity;
    }
}
