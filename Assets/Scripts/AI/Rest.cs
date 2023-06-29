using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rest : GAction
{
    public override bool PrePerform()
    {
        Target = GWorld.Instance.GetQueue(GWorld.REST_LOCATIONS).RemoveResource();

        if (Target == null) { return false; }

        Inventory.AddItem(Target);

        GWorld.Instance.GetWorld().ModifyState(GWorld.FREE_REST_LOCATIONS, -1);

        return true;
    }

    public override bool Perform()
    {
        base.Perform();

        return true;
    }

    public override bool PostPeform()
    {
        GWorld.Instance.GetQueue(GWorld.REST_LOCATIONS).AddResource(Target);

        Inventory.RemoveItem(Target);

        GWorld.Instance.GetWorld().ModifyState(GWorld.FREE_REST_LOCATIONS, 1);

        Beliefs.RemoveState("needToRest");

        return true;
    }
}
