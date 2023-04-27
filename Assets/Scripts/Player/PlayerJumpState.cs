using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(BaseStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {
        _isRootState = true;
        InitializeSubStates();
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
        if (_context.PlayerHealth.IsHurt || _context.PlayerHealth.IsDead())
        {
            SwitchState(_factory.Ragdoll());
        }
        else if (_context.IsGrounded)
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
        if (_context.InteractionController.HasRecentlyInteracted()) { return; }

        _context.Anim.SetTrigger("Jump");

        //float jumpingVelocity = Mathf.Sqrt(-2 * _context.GravityIntensity * _context.JumpHeight * Time.deltaTime);
        //Vector3 playerVelocity = _context.MoveDirection;
        //playerVelocity.y = jumpingVelocity;
        PlayerStateMachine stateMachine = _context as PlayerStateMachine;
        //stateMachine.FallVelocity = playerVelocity;
        stateMachine.FallSpeed = _context.JumpHeight;
    }
}
