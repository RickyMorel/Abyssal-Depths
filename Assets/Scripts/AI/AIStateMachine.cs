using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIStateMachine : BaseStateMachine
{
    #region Private Variables

    private NavMeshAgent _agent;
    private AICombat _aiCombat;
    private bool _isBouncingOffShield = false;
    private float _movementSpeed = 1f;

    #endregion

    #region Public Properties

    public NavMeshAgent Agent => _agent;
    public AICombat AICombat => _aiCombat;
    public bool IsBouncingOffShield => _isBouncingOffShield;

    #endregion

    public override void Start()
    {
        base.Start();

        _agent = GetComponent<NavMeshAgent>();
        _aiCombat = GetComponent<AICombat>();
    }

    public override void Move()
    {
        _moveDirection.x = _canMove ? _movementSpeed : 0f;
    }

    public void BasicAttack()
    {
        if (!CanMove) { return; }

        StartCoroutine(SetIsShootingCoroutine());
    }

    public void Attack(int attackNumber, bool checkCanMove = true)
    {
        if (checkCanMove) { if (!CanMove) { return; } }

        Anim.SetInteger("AttackType", attackNumber);
        Anim.SetTrigger("Attack");
    }

    public void ResetAttacking()
    {
        Anim.SetInteger("AttackType", 0);
        Anim.ResetTrigger("Attack");
    }

    public void SetIsCarryingItem(bool isCarrying)
    {
        Anim.SetBool("IsCarryingItem", isCarrying);
    }

    public IEnumerator SetIsShootingCoroutine()
    {
        _isShooting = true;

        yield return new WaitForSeconds(0.5f);

        _isShooting = false;
    }

    public void SetRagdollState(bool isEnabled)
    {
        if (isEnabled)
        {
            _isBouncingOffShield = isEnabled;
        }
        //Disables enemy ragdoll once it touches the floor
        else
        {
            Debug.Log("SetRagdollState False");
            StartCoroutine(SetBouncingOffShieldCoroutine(Vector3.zero, 0f));
        }
    }

    public void BounceOffShield(Vector3 pushDir, float pushForce)
    {
        if (_isBouncingOffShield) { return; }

        _isBouncingOffShield = true;

        StartCoroutine(SetBouncingOffShieldCoroutine(pushDir, pushForce));
    }

    public IEnumerator SetBouncingOffShieldCoroutine(Vector3 pushDir, float pushForce)
    {
        yield return new WaitForEndOfFrame();

        if (_rb.velocity.magnitude < pushForce * 0.8f)
        {
            _rb.AddForce(pushDir.normalized * _rb.mass * pushForce, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(3f);

        if (!IsOnGround()) { StartCoroutine(SetBouncingOffShieldCoroutine(pushDir, pushForce)); }
        else { _isBouncingOffShield = false; }
    }

    public void SetMovementSpeed(float newSpeed)
    {
        _movementSpeed = newSpeed;
    }

    private bool IsOnGround()
    {
        NavMeshHit hit;

        bool isOnNavMesh = NavMesh.SamplePosition(transform.position, out hit, 5f, _agent.areaMask);

        return isOnNavMesh;
    }
}
