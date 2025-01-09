using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class A_Chase : A_Base
{
	#region Editor Fields

	[SerializeField] private float _attackDistance = 3f;

	#endregion

	#region Private Variables

	private AI_ShootController _aiShootController;
	private NavMeshAgent _agent;
	private bool _isAttacking;

	#endregion

	public override void Start()
	{
		base.Start();

		_agent = GetComponent<NavMeshAgent>();
		_aiShootController = GetComponent<AI_ShootController>();
	}

	public override void StartAction()
	{
		base.StartAction();
	}

	public override void DoAction()
	{
		Debug.Log("Do Chase");
		if (_isAttacking) { return; }
		Debug.Log("Do Chase is not attacking");

		CheckSwitchAction();

		if (!_aiCues.Ship) { return; }
		Debug.Log("Do Chase knows where ship is");

		ChasePlayer();
	}

	private void ChasePlayer()
	{
		Debug.Log("_aiCues.Ship: " + _aiCues.Ship.gameObject.name);

		float distanceFromPlayer = Vector3.Distance(_aiCues.Ship.transform.position, transform.position);

		if (distanceFromPlayer <= _attackDistance) { StartCoroutine(AttackPlayer()); return; }

		_agent.SetDestination(_aiCues.Ship.transform.position);
	}

	private IEnumerator AttackPlayer()
	{
		_isAttacking = true;

		Debug.Log("ATTACK!");

		_agent.isStopped = true;

		_aiStateMachine.Anim.Play("Attack", 0);

		yield return new WaitForSeconds(1f);

		_isAttacking = false;

		_agent.isStopped = false;

		_agent.ResetPath();
	}

	public override void CheckSwitchAction()
	{
		base.CheckSwitchAction();

		if (!_aiCues.Ship) { _aiStateMachine.DoPatrol(); }
	}
}