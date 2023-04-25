using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellStuff : GAction
{
    [SerializeField] private GameObject _shop;

    public override bool PrePerform()
    {
        Target = _shop;

        if (Target == null) { return false; }

        Inventory.AddItem(Target);

        return true;
    }

    public override bool Perform()
    {
        return true;
    }

    public override bool PostPeform()
    {
        Inventory.RemoveItem(Target);

        Beliefs.RemoveState("wantToSell");

        return true;
    }
}
