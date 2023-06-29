using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachAttack : GAction
{
    #region Editor Fields

    [SerializeField] private GWorld.AttackTags _attackItemTag;
    [SerializeField] private GWorld.AttackFreeTags _attackFreeItemName;

    #endregion

    public override bool PrePerform()
    {
        Target = GWorld.Instance.GetQueue(_attackItemTag.ToString()).RemoveResource();

        if (Target == null) { return false; }

        Inventory.AddItem(Target);

        GWorld.Instance.GetWorld().ModifyState(_attackFreeItemName.ToString(), -1);

        return true;
    }

    public override bool Perform()
    {
        base.Perform();

        return true;
    }

    public override bool PostPeform()
    {
        GWorld.Instance.GetQueue(_attackItemTag.ToString()).AddResource(Target);

        Inventory.RemoveItem(Target);

        GWorld.Instance.GetWorld().ModifyState(_attackFreeItemName.ToString(), 1);

        Beliefs.RemoveState("hurt");

        return true;
    }
}
