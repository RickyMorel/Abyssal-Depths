using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowRock : GAction
{
    #region Editor Fields

    [SerializeField] private GWorld.AttackTags _attackItemTag;
    [SerializeField] private GWorld.AttackFreeTags _attackFreeItemName;

    #endregion

    #region Private Variables

    private AIStateMachine _stateMachine;
    private GAgent _gAgent;
    private AICombat _aiCombat;

    #endregion

    private void Start()
    {
        _gAgent = Agent.GetComponent<GAgent>();
        _stateMachine = Agent.GetComponent<AIStateMachine>();
        _aiCombat = Agent.GetComponent<AICombat>();
    }

    public override bool PrePerform()
    {
        Target = GWorld.Instance.GetQueue(_attackItemTag.ToString()).RemoveResource();

        if (Target == null) { return false; }

        Inventory.AddItem(Target);

        GWorld.Instance.GetWorld().ModifyState(_attackFreeItemName.ToString(), -1);

        _gAgent.SetGoalDistance(_aiCombat.AttackRange);

        return true;
    }

    public override bool PostPeform()
    {
        GWorld.Instance.GetQueue(_attackItemTag.ToString()).AddResource(Target);

        Inventory.RemoveItem(Target);

        GWorld.Instance.GetWorld().ModifyState(_attackFreeItemName.ToString(), 1);

        _stateMachine.Attack(3);

        Beliefs.RemoveState("hasRock");

        _gAgent.ResetGoalDistance();

        return true;
    }
}