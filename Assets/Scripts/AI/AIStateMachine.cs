using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIStateMachine : BaseStateMachine
{
    #region Private Variables

    private NavMeshAgent _agent;
    private bool _isBouncingOffShield = false;

    #endregion

    #region Public Properties

    public NavMeshAgent Agent => _agent;
    public bool IsBouncingOffShield => _isBouncingOffShield;

    #endregion

    public override void Start()
    {
        base.Start();

        _agent = GetComponent<NavMeshAgent>();
    }

    public override void Move()
    {
        _moveDirection.x = _canMove ? 1f : 0f;
    }

    public void BasicAttack()
    {
        if (!CanMove) { return; }

        StartCoroutine(SetIsShootingCoroutine());
    }

    public void BounceOffShield()
    {
        if (_isBouncingOffShield) { return; }

        _isBouncingOffShield = true;

        StartCoroutine(SetBouncingOffShieldCoroutine());
    }

    public IEnumerator SetIsShootingCoroutine()
    {
        _isShooting = true;

        yield return new WaitForSeconds(0.5f);

        _isShooting = false;
    }

    public IEnumerator SetBouncingOffShieldCoroutine()
    {
        _isBouncingOffShield = true;

        yield return new WaitForSeconds(3f);

        if (!IsOnGround()) { StartCoroutine(SetBouncingOffShieldCoroutine()); }
        else { _isBouncingOffShield = false; }
    }

    private bool IsOnGround()
    {
        NavMeshHit hit;

        bool isOnNavMesh = NavMesh.SamplePosition(transform.position, out hit, 5f, _agent.areaMask);

        return isOnNavMesh;
    }
}
