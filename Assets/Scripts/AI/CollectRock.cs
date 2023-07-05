using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectRock : GAction
{
    public override void Start()
    {
        base.Start();
    }

    public override bool PrePerform()
    {
        Debug.Log("PrePerform Collect Rock");
        Target = GWorld.Instance.GetQueue(GWorld.ROCK_PICKUP_POINTS).RemoveResource();

        GAgent.StateMachine.AICombat.DestroyPrevHeldProjectile();

        if (Target == null) { return false; }

        Inventory.AddItem(Target);

        GWorld.Instance.GetWorld().ModifyState(GWorld.FREE_ROCK_PICKUP_POINTS, -1);

        return true;
    }

    public override bool Perform()
    {
        base.Perform();

        return true;
    }

    public override bool PostPeform()
    {
        Debug.Log("PostPeform Collect Rock");
        GWorld.Instance.GetQueue(GWorld.ROCK_PICKUP_POINTS).AddResource(Target);

        Inventory.RemoveItem(Target);

        GWorld.Instance.GetWorld().ModifyState(GWorld.FREE_ROCK_PICKUP_POINTS, 1);

        GAgent.StateMachine.SetIsCarryingItem(true);

        Beliefs.RemoveState("getRock");

        return true;
    }
}
