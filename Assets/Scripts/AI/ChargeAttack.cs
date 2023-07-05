using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;
using UnityEngine.AI;

public class ChargeAttack : GAction
{
    #region Editor Fields

    [SerializeField] private float _chargeStopDistance;
    [SerializeField] private float _chargeSpeed;
    [SerializeField] private LayerMask _chargeStopMask;

    #endregion

    #region Private Variables

    private float _defaultSpeed;
    private float _timeSinceDoAction;
    private ObstacleAvoidanceType _defaultObstacleAvoidance;

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

        if(GAgent.CurrentAction != this) { return; }

        DoHeadButtAnimation();
    }

    public override bool PrePerform()
    {
        Debug.Log("PrePerform Charge");
        GAgent.StateMachine.SetMovementSpeed(2f);
        Agent.speed = _chargeSpeed;
        GAgent.SetGoalDistance(_chargeStopDistance);

        GAgent.StateMachine.AICombat.DestroyPrevHeldProjectile();

        _timeSinceDoAction = 0f;

        Target = Ship.Instance.gameObject;

        if (Target == null) { return false; }

        return true;
    }

    private void Update()
    {
        if (GAgent.CurrentAction != this) { return; }

        _timeSinceDoAction += Time.deltaTime;

        if (_timeSinceDoAction < 8f) { return; }

        GAgent.CancelPreviousActions();
    }

    public override bool Perform()
    {
        base.Perform();

        return true;
    }

    private void DoHeadButtAnimation()
    {
        GAgent.StateMachine.Anim.SetInteger("AttackType", 2);
        GAgent.StateMachine.Anim.SetTrigger("Attack");

        Invoke(nameof(FinishCharge), 2f);
    }

    public void RotatedMessage()
    {
        //if(GAgent.CurrentAction is not ChargeAttack) { return; }

        //FinishCharge();
    }

    private void FinishCharge()
    {
        GAgent.CancelPreviousActions();
    } 

    public override bool PostPeform()
    {
        Debug.Log("PostPeform Charge");
        GAgent.StateMachine.SetMovementSpeed(1f);
        Agent.speed = _defaultSpeed;
        Agent.obstacleAvoidanceType = _defaultObstacleAvoidance;
        GAgent.ResetGoalDistance();

        Beliefs.RemoveState("charge");
        Beliefs.RemoveState("hasDistance");

        return true;
    }
}
