using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MegalodonRagdollAttack : GAction
{
    #region Editor Fields

    [SerializeField] private Transform _mouthTransform;
    [SerializeField] private Collider _biteCollider;
    [SerializeField] private Collider _bodyCollider;
    [SerializeField] private float _attackSpeed = 50f;

    #endregion

    #region Private Variables

    private GameObject _currentRunAwayObj;
    private bool _goToZPos;
    private bool _prevIsKinematic;
    private bool _prevUseGravity;
    private bool _hitShip;

    #endregion

    public override bool PrePerform()
    {
        _hitShip = false;

        _prevUseGravity = GAgent.StateMachine.Rb.useGravity;
        _prevIsKinematic = GAgent.StateMachine.Rb.isKinematic;

        _biteCollider.enabled = true;
        _bodyCollider.enabled = false;

        GAgent.StateMachine.Rb.isKinematic = true;
        GAgent.StateMachine.Rb.useGravity = false;
        GAgent.StateMachine.CanMove = false;

        GameObject newTargetObj = new GameObject();
        newTargetObj.transform.position = Ship.Instance.transform.position;
        newTargetObj.name = "MegalodonZPosTarget";

        _currentRunAwayObj = newTargetObj;

        Target = newTargetObj;

        if (Target == null) { return false; }

        _goToZPos = true;

        return true;
    }

    public void BiteShip(Collider collider)
    {
        Debug.Log("BiteShip try");

        if (GAgent.CurrentAction is not MegalodonRagdollAttack) { return; }

        if(collider.gameObject.tag != "MainShip") { return; }

        Debug.Log("BiteShip 2");

        Ship.Instance.FreezeShip(true);

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
        if(_currentRunAwayObj == null) { return; }

        var step = _attackSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _currentRunAwayObj.transform.position, step);
    }

    public override bool Perform()
    {
        if (!_hitShip) { return true; }

        GAgent.StateMachine.Attack(5, false);
        Ship.Instance.transform.position = _mouthTransform.position;

        return true;
    }

    public override bool PostPeform()
    {
        if(_currentRunAwayObj != null) { Destroy(_currentRunAwayObj); }

        GAgent.StateMachine.Rb.isKinematic = _prevIsKinematic;
        GAgent.StateMachine.Rb.useGravity = _prevUseGravity;
        _bodyCollider.enabled = true;
        _goToZPos = false;

        GAgent.StateMachine.ResetAttacking();

        Debug.Log("PostPerform MegalodonRagdollAttack");

        Ship.Instance.FreezeShip(false);

        return true;
    }
}
