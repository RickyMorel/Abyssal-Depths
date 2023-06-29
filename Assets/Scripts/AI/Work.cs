using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Work : GAction
{
    [SerializeField] private GameObject _workLocation;

    public override bool PrePerform()
    {
        Target = _workLocation;

        if (Target == null) { return false; }

        Inventory.AddItem(Target);

        return true;
    }

    public override bool Perform()
    {
        base.Perform();

        return true;
    }

    public override bool PostPeform()
    {
        Inventory.RemoveItem(Target);

        Beliefs.RemoveState("wantToWork");

        return true;
    }
}
