using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChargeAttack : GAction
{
    #region Editor Fields

    [SerializeField] private float _chargeSpeed;
    [SerializeField] private LayerMask _chargeStopMask;

    #endregion

    #region Private Variables

    private GameObject _currentChargeObj;
    private float _defaultSpeed;
    private ObstacleAvoidanceType _defaultObstacleAvoidance;
    private Vector3 _destination;

    #endregion

    public override void Start()
    {
        base.Start();

        _defaultSpeed = Agent.speed;
        _defaultObstacleAvoidance = Agent.obstacleAvoidanceType;
    }

    //Start playing headbutt animation before you reach the target
    public void OnChargeTrigger(Collider other)
    {
        if(other.gameObject.tag != "MainShip") { return; }

        DoHeadButtAnimation();
    }

    public override bool PrePerform()
    {
        GAgent.StateMachine.SetMovementSpeed(2f);
        Agent.speed = _chargeSpeed;
        Agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

        _destination = Ship.Instance.transform.position - (Ship.Instance.transform.position - transform.position).normalized * 5;

        GameObject newTargetObj = new GameObject();
        newTargetObj.transform.position = _destination;
        newTargetObj.name = "ChargeTargetPositionObj";

        _currentChargeObj = newTargetObj;

        Target = newTargetObj;

        if (Target == null) { return false; }

        return true;
    }

    public override bool Perform()
    {
        return true;
    }

    private void DoHeadButtAnimation()
    {
        Debug.Log("DoHeadButtAnimation");
        GAgent.StateMachine.Anim.SetInteger("AttackType", 2);
        GAgent.StateMachine.Anim.SetTrigger("Attack");
    }

    public override bool PostPeform()
    {
        GAgent.StateMachine.SetMovementSpeed(1f);
        Agent.speed = _defaultSpeed;
        Agent.obstacleAvoidanceType = _defaultObstacleAvoidance;

        if (_currentChargeObj != null) { Destroy(_currentChargeObj); }

        Beliefs.RemoveState("charge");

        _destination = Vector3.zero;

        return true;
    }
}
