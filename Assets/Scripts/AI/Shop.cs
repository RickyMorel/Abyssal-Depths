using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : GAction
{
    public override bool PrePerform()
    {
        Target = GWorld.Instance.GetQueue(GWorld.SHOPS).RemoveResource();

        if (Target == null) { return false; }

        Inventory.AddItem(Target);

        GWorld.Instance.GetWorld().ModifyState(GWorld.FREE_SHOPS, -1);

        return true;
    }

    public override bool Perform()
    {
        return true;
    }

    public override bool PostPeform()
    {
        GWorld.Instance.GetQueue(GWorld.SHOPS).AddResource(Target);

        Inventory.RemoveItem(Target);

        GWorld.Instance.GetWorld().ModifyState(GWorld.FREE_SHOPS, 1);

        Beliefs.RemoveState("needToShop");

        return true;
    }
}
