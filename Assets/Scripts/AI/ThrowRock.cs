using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowRock : GAction
{
    #region Editor Fields

    [SerializeField] private float _throwRange;
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
        base.Start();

        _gAgent = Agent.GetComponent<GAgent>();
        _stateMachine = Agent.GetComponent<AIStateMachine>();
        _aiCombat = Agent.GetComponent<AICombat>();
    }

    public override bool PrePerform()
    {
        Debug.Log("PrePerform Throw Rock");
        Target = GWorld.Instance.GetQueue(_attackItemTag.ToString()).RemoveResource();

        if (Target == null) { return false; }

        Debug.Log("PrePerform Throw Rock Target Not Null");

        Inventory.AddItem(Target);

        GWorld.Instance.GetWorld().ModifyState(_attackFreeItemName.ToString(), -1);

        _gAgent.SetGoalDistance(_throwRange);

        return true;
    }

    public override bool Perform()
    {
        base.Perform();

        _stateMachine.Attack(3);

        GAgent.StateMachine.SetIsCarryingItem(false);

        return true;
    }

    public override bool PostPeform()
    {
        GWorld.Instance.GetQueue(_attackItemTag.ToString()).AddResource(Target);

        Inventory.RemoveItem(Target);

        GWorld.Instance.GetWorld().ModifyState(_attackFreeItemName.ToString(), 1);

        Beliefs.RemoveState("hasRock");

        _gAgent.ResetGoalDistance();

        GAgent.StateMachine.AICombat.DestroyPrevHeldProjectile();

        return true;
    }
}