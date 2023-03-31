using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : GAction
{
    public override bool PrePerform()
    {
        Target = GWorld.Instance.GetQueue(GWorld.EATINGCHAIRS).RemoveResource();

        if (Target == null) { return false; }

        Inventory.AddItem(Target);

        GWorld.Instance.GetWorld().ModifyState(GWorld.FREE_EATINGCHAIR, -1);

        return true;
    }

    public override bool Perform()
    {
        return true;
    }

    public override bool PostPeform()
    {
        GWorld.Instance.GetQueue(GWorld.EATINGCHAIRS).AddResource(Target);

        Inventory.RemoveItem(Target);

        GWorld.Instance.GetWorld().ModifyState(GWorld.FREE_EATINGCHAIR, 1);

        Beliefs.RemoveState("hungry");

        return true;
    }
}
