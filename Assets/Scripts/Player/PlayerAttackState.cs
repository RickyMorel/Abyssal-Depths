using log4net.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    #region Private Variables

    private float _timeSinceLastAttack;
    private float _timeBetweenAttacks = 0.5f;

    #endregion

    public PlayerAttackState(BaseStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        if (_context.InteractionController.IsInteracting()) { _timeSinceLastAttack = 0; return; }

        _context.Speed = 0;

        Attack();
    }

    public override void UpdateState()
    {
        CheckSwitchStates();

        _timeSinceLastAttack += Time.deltaTime;
    }

    public override void ExitState() { }

    public override void InitializeSubStates() { }

    public override void CheckSwitchStates()
    {
        if(_timeSinceLastAttack < _timeBetweenAttacks) { return; }

        SwitchState(_factory.Idle());
    }

    private void Attack()
    {
        _timeSinceLastAttack = 0f;

        _context.Anim.SetTrigger("Attack");
    }
}
