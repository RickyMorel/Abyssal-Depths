using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hide : GAction
{
    public override bool PrePerform()
    {
        Target = GWorld.Instance.GetQueue(GWorld.HIDE_LOCATIONS).RemoveResource();

        if (Target == null) { return false; }

        Inventory.AddItem(Target);

        GWorld.Instance.GetWorld().ModifyState(GWorld.FREE_HIDE_LOCATIONS, -1);

        return true;
    }

    public override bool Perform()
    {
        base.Perform();

        return true;
    }

    public override bool PostPeform()
    {
        GWorld.Instance.GetQueue(GWorld.HIDE_LOCATIONS).AddResource(Target);

        Inventory.RemoveItem(Target);

        GWorld.Instance.GetWorld().ModifyState(GWorld.FREE_HIDE_LOCATIONS, 1);

        //Beliefs.RemoveState("hungry");

        return true;
    }
}
