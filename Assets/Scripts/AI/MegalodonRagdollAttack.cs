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
    [SerializeField] private float _throwForce = 50f;

    #endregion

    #region Private Variables

    private bool _goToZPos;
    private bool _prevIsKinematic;
    private bool _prevUseGravity;
    private bool _hitShip;
    private Vector3 _prevSize;
    private Vector3 _targetOriginalPosition;

    #endregion

    public override bool PrePerform()
    {
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

        //Get's the target's starting position so megalodon does not lock on to ship when chasing, this allows the player to dodge the attack
        _targetOriginalPosition = Target.transform.position;

        _goToZPos = true;

        GAgent.StateMachine.Attack(5, false);

        return true;
    }

    public void BiteShip()
    {
        if (GAgent.CurrentAction is not MegalodonRagdollAttack) { return; }

        //Position the megalodon correctly to bite ship
        transform.position = Target.transform.position;
        transform.rotation = Target.transform.rotation;

        Ship.Instance.FreezeShip(true);

        _goToZPos = false;

        _hitShip = true;

        _biteCollider.enabled = false;
    }

    private void Update()
    {
        GetToZPos();
    }

    private void LateUpdate()
    {
        if (!_hitShip) { return; }

        Ship.Instance.transform.position = _mouthTransform.transform.position;
    }

    private void GetToZPos()
    {
        if (_goToZPos == false) { return; }

        var step = _attackSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _targetOriginalPosition, step);

        if(Vector3.Distance(transform.position, _targetOriginalPosition) < 2f) { _goToZPos = false; CheckIfDidntHitPlayer(); }
    }

    public override bool Perform()
    {
        base.Perform();

        return true;
    }

    public override bool PostPeform()
    {
        //close mouth
        GAgent.StateMachine.Attack(0, false);

        //Enable hitBox after 4 seconds to prevent shield from hitting megalodon
        Invoke(nameof(EnableBodyCollider), 4f);

        GAgent.StateMachine.ResetAttacking();

        GWorld.Instance.GetQueue(_attackItemTag.ToString()).AddResource(Target);

        Inventory.RemoveItem(Target);

        GWorld.Instance.GetWorld().ModifyState(_attackFreeItemName.ToString(), 1);

        Ship.Instance.FreezeShip(false);
        //Only ragdoll ship in megalodon bit it
        if (_hitShip) { Ship.Instance.Rb.AddForce(Vector3.right * _throwForce, ForceMode.VelocityChange); }

        _hitShip = false;

        transform.localScale = _prevSize;

        GAgent.StateMachine.CanMove = true;

        return true;
    }

    private void CheckIfDidntHitPlayer()
    {
        if(_goToZPos == false && _hitShip == false)
        {
            GAgent.CancelPreviousActions();
        }
    }

    private void EnableBodyCollider()
    {
        _bodyCollider.enabled = true;
        _biteCollider.enabled = false;

        GAgent.StateMachine.Rb.isKinematic = _prevIsKinematic;
        GAgent.StateMachine.Rb.useGravity = _prevUseGravity;
    }
}
