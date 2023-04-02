using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChargeAttack : GAction
{
    #region Editor Fields

    [SerializeField] private float _chargeSpeed;

    #endregion

    #region Private Variables

    private GameObject _currentChargeObj;
    private float _defaultSpeed;
    private ObstacleAvoidanceType _defaultObstacleAvoidance;

    #endregion

    public override void Start()
    {
        base.Start();

        _defaultSpeed = Agent.speed;
        _defaultObstacleAvoidance = Agent.obstacleAvoidanceType;
    }

    public override bool PrePerform()
    {
        GAgent.StateMachine.SetMovementSpeed(2f);
        Agent.speed = _chargeSpeed;
        Agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

        Vector3 destination = Ship.Instance.transform.position + (Ship.Instance.transform.position - transform.position).normalized * 20;

        if (Physics.Raycast(destination, transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity))
        {
            destination = hit.point;
        }

        GameObject newTargetObj = new GameObject();
        newTargetObj.transform.position = destination;
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

    public override bool PostPeform()
    {
        GAgent.StateMachine.SetMovementSpeed(1f);
        Agent.speed = _defaultSpeed;
        Agent.obstacleAvoidanceType = _defaultObstacleAvoidance;

        if (_currentChargeObj != null) { Destroy(_currentChargeObj); }

        Beliefs.RemoveState("charge");

        return true;
    }
}
