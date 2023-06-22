using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectRock : GAction
{
    private float _timeSinceDoAction;

    public override void Start()
    {
        base.Start();
    }

    public override bool PrePerform()
    {
        Target = GWorld.Instance.GetQueue(GWorld.ROCK_PICKUP_POINTS).RemoveResource();

        if (Target == null) { return false; }

        Inventory.AddItem(Target);

        GWorld.Instance.GetWorld().ModifyState(GWorld.FREE_ROCK_PICKUP_POINTS, -1);

        return true;
    }

    private void Update()
    {
        if (GAgent.CurrentAction != this) { return; }

        _timeSinceDoAction += Time.deltaTime;

        if (_timeSinceDoAction < 10f) { return; }

        GAgent.CancelPreviousActions();
    }

    public override bool Perform()
    {
        return true;
    }

    public override bool PostPeform()
    {
        GWorld.Instance.GetQueue(GWorld.ROCK_PICKUP_POINTS).AddResource(Target);

        Inventory.RemoveItem(Target);

        GWorld.Instance.GetWorld().ModifyState(GWorld.FREE_ROCK_PICKUP_POINTS, 1);

        GAgent.StateMachine.SetIsCarryingItem(true);

        Beliefs.RemoveState("getRock");

        _timeSinceDoAction = 0f;

        return true;
    }
}
