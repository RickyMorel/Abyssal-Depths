using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MegalodonFinAttack : GAction
{
    #region Editor Fields

    [Header("Stats")]
    [SerializeField] private float _finAttackDistance;
    [SerializeField] private float _finChaseSpeed;
    [SerializeField] private float _pushForce;
    [SerializeField] private Collider _finAttackZoneCollider;

    [Header("FX")]
    [SerializeField] private ParticleSystem _finAttackParticles;

    #endregion

    #region Private Variables

    private GameObject _currentTargetObj;
    private float _defaultSpeed;
    private float _initialZPosition;

    #endregion

    public override void Start()
    {
        base.Start();

        _defaultSpeed = Agent.speed;
        _initialZPosition = transform.position.z;
    }

    //Start playing headbutt animation before you reach the target
    public void OnEnterAttackArea(Collider other)
    {
        if (GAgent.CurrentAction != this) { return; }

        Vector3 pushDir = Ship.Instance.transform.position - transform.position;

        //Send ship flying backwards
        if (other.gameObject.tag == "MainShip") { Ship.Instance.Rb.AddForce(_pushForce * 1000f * pushDir * Time.deltaTime, ForceMode.Force); }

        //Send projectiles flying back as well
        if(other.gameObject.TryGetComponent(out Projectile projectile))
        {
            projectile.Rb.AddForce(_pushForce * pushDir * Time.deltaTime, ForceMode.Force);
            projectile.transform.LookAt(pushDir);
            projectile.Initialize("Enemy");
        }
    }

    public override bool PrePerform()
    {
        GAgent.StateMachine.SetMovementSpeed(1.5f);
        Agent.speed = _finChaseSpeed;
        GAgent.SetGoalDistance(_finAttackDistance);
        GAgent.StateMachine.CanMove = true;

        GameObject newTargetObj = new GameObject();
        newTargetObj.transform.position = Ship.Instance.transform.position;
        newTargetObj.name = "FinAttackPositionObj";

        _currentTargetObj = newTargetObj;

        Target = newTargetObj;

        if (Target == null) { return false; }

        return true;
    }

    public override bool Perform()
    {
        base.Perform();

        GAgent.StateMachine.Attack(4);

        //Prevents Megalodon from moving in the Z plane
        transform.position = new Vector3(transform.position.x, transform.position.y, _initialZPosition);

        return true;
    }

    public void EnableFinAttackCollider()
    {
        _finAttackZoneCollider.enabled = true;

        _finAttackParticles.Play();
    }

    public override bool PostPeform()
    {
        if (_currentTargetObj != null) { Destroy(_currentTargetObj); }

        GAgent.StateMachine.SetMovementSpeed(1f);
        Agent.speed = _defaultSpeed;
        GAgent.ResetGoalDistance();
        GAgent.StateMachine.ResetAttacking();

        _finAttackZoneCollider.enabled = false;
        _finAttackParticles.Stop();

        return true;
    }
}
