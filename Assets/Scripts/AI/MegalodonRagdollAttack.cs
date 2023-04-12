using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MegalodonRagdollAttack : GAction
{
    #region Editor Fields

    [SerializeField] private GWorld.AttackTags _attackItemTag;
    [SerializeField] private GWorld.AttackFreeTags _attackFreeItemName;

    [SerializeField] private Transform _mouthTransform;
    [SerializeField] private Collider _biteCollider;
    [SerializeField] private Collider _bodyCollider;
    [SerializeField] private float _attackSpeed = 50f;

    #endregion

    #region Private Variables

    private bool _goToZPos;
    private bool _prevIsKinematic;
    private bool _prevUseGravity;
    private bool _hitShip;
    private Vector3 _prevSize;

    #endregion

    public override bool PrePerform()
    {
        Debug.Log("PrePerform");

        _hitShip = false;
        _prevSize = transform.localScale;

        transform.localScale = new Vector3(12, 12, 12);

        _prevUseGravity = GAgent.StateMachine.Rb.useGravity;
        _prevIsKinematic = GAgent.StateMachine.Rb.isKinematic;

        _biteCollider.enabled = true;
        _bodyCollider.enabled = false;

        GAgent.StateMachine.Rb.isKinematic = true;
        GAgent.StateMachine.Rb.useGravity = false;
        GAgent.StateMachine.CanMove = false;

        Target = GWorld.Instance.GetQueue(_attackItemTag.ToString()).RemoveResource();

        if (Target == null) { return false; }

        Inventory.AddItem(Target);

        GWorld.Instance.GetWorld().ModifyState(_attackFreeItemName.ToString(), -1);

        _goToZPos = true;

        return true;
    }

    public void BiteShip()
    {
        Debug.Log("BiteShip");
        if (GAgent.CurrentAction is not MegalodonRagdollAttack) { return; }

        Ship.Instance.FreezeShip(true);

        _goToZPos = false;

        _hitShip = true;

        _biteCollider.enabled = false;
    }

    private void Update()
    {
        if(_goToZPos == false) { return; }

        GetToZPos();
    }

    private void GetToZPos()
    {
        //if(Target == null) { return; }

        var step = _attackSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, step);
    }

    public override bool Perform()
    {
        Debug.Log("Perform");

        if (!_hitShip) { return true; }

        GAgent.StateMachine.Attack(5, false);

        //Vector3 finalShipPos = new Vector3(_mouthTransform.position.x, _mouthTransform.position.y, Ship.Instance.transform.position.z);
        //Ship.Instance.transform.position = finalShipPos;

        return true;
    }

    public override bool PostPeform()
    {
        GAgent.StateMachine.Rb.isKinematic = _prevIsKinematic;
        GAgent.StateMachine.Rb.useGravity = _prevUseGravity;
        //Enable hitBox after 4 seconds to prevent shield from hitting megalodon
        Invoke(nameof(EnableBodyCollider), 4f);

        GAgent.StateMachine.ResetAttacking();

        GWorld.Instance.GetQueue(_attackItemTag.ToString()).AddResource(Target);

        Inventory.RemoveItem(Target);

        GWorld.Instance.GetWorld().ModifyState(_attackFreeItemName.ToString(), 1);

        Debug.Log("PostPerform MegalodonRagdollAttack");

        Ship.Instance.FreezeShip(false);

        transform.localScale = _prevSize;

        return true;
    }

    private void EnableBodyCollider()
    {
        _bodyCollider.enabled = true;
    }
}
