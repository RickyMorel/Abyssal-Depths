using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackShip : GAction
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

    public override void Start()
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

    public override bool Perform()
    {
        base.Perform();

        return true;
    }

    public override bool PostPeform()
    {
        GWorld.Instance.GetQueue(_attackItemTag.ToString()).AddResource(Target);

        Inventory.RemoveItem(Target);

        GWorld.Instance.GetWorld().ModifyState(_attackFreeItemName.ToString(), 1);

        _stateMachine.BasicAttack();

        _gAgent.ResetGoalDistance();

        return true;
    }
}