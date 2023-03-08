using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIStateMachine : BaseStateMachine
{
    #region Private Variables

    private GAgent _gAgent;
    private NavMeshAgent _agent;

    #endregion

    #region Public Properties

    public NavMeshAgent Agent => _agent;

    #endregion

    public override void Start()
    {
        base.Start();

        _gAgent = GetComponent<GAgent>();
        _agent = GetComponent<NavMeshAgent>();
    }

    public override void Move()
    {
        _moveDirection.x = _gAgent.IsMoving ? 1f : 0f;
    }

    public void BasicAttack()
    {
        if (!CanMove) { return; }

        StartCoroutine(SetIsShootingCoroutine());
    }

    public IEnumerator SetIsShootingCoroutine()
    {
        _isShooting = true;

        yield return new WaitForSeconds(0.5f);

        _isShooting = false;
    }
}
