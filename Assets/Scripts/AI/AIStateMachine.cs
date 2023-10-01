using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(GAgent))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AICombat))]
[RequireComponent(typeof(AIAudio))]
[RequireComponent(typeof(AIHealth))]
public class AIStateMachine : BaseStateMachine
{
    #region Private Variables

    private GAgent _gAgent;
    private NavMeshAgent _agent;
    private AICombat _aiCombat;
    private AIHealth _aiHealth;
    private AIAudio _aiAudio;
    [SerializeField] private bool _isBouncingOffShield = false;
    private float _movementSpeed = 1f;

    #endregion

    #region Public Properties

    public NavMeshAgent Agent => _agent;
    public AIAudio AIAudio => _aiAudio;
    public AICombat AICombat => _aiCombat;
    public AIHealth AIHealth => _aiHealth;
    public bool IsBouncingOffShield => _isBouncingOffShield;

    #endregion

    public override void Start()
    {
        base.Start();

        _agent = GetComponent<NavMeshAgent>();
        _aiCombat = GetComponent<AICombat>();
        _gAgent = GetComponent<GAgent>();
        _aiAudio = GetComponent<AIAudio>();
        _aiHealth = GetComponent<AIHealth>();
    }

    public override void Move()
    {
        _moveDirection.x = _gAgent.IsMoving() ? _movementSpeed : 0f;
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

        _aiHealth.StopAllCoroutines();

        yield return new WaitForSeconds(1f);

        while(!IsOnGround(out NavMeshHit groundPos))
        {
            yield return null;
        }

        IsOnGround(out NavMeshHit groundPosConfirmed);

        transform.position = groundPosConfirmed.position;
        transform.rotation = Quaternion.identity;

        _isBouncingOffShield = false;

        StopAllCoroutines();
        _aiHealth.SetCanMove(true);
       // StartCoroutine(_aiHealth.PushWhenShot());
    }

    public void SetMovementSpeed(float newSpeed)
    {
        _movementSpeed = newSpeed;
    }

    private bool IsOnGround(out NavMeshHit groundPos)
    {
        NavMeshHit hit;

        bool isOnNavMesh = NavMesh.SamplePosition(transform.position, out hit, 5f, _agent.areaMask);

        groundPos = hit;

        return isOnNavMesh;
    }
}
