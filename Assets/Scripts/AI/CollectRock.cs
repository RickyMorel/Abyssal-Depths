using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectRock : GAction
{
    public override bool PrePerform()
    {
        Target = GWorld.Instance.GetQueue(GWorld.ROCK_PICKUP_POINTS).RemoveResource();

        if (Target == null) { return false; }

        Inventory.AddItem(Target);

        GWorld.Instance.GetWorld().ModifyState(GWorld.FREE_ROCK_PICKUP_POINTS, -1);

        return true;
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

        Beliefs.RemoveState("getRock");

        return true;
    }
}
