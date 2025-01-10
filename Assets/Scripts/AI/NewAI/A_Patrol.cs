using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class A_Patrol : A_Base
{
	#region Editor Fields

	[SerializeField] private List<Transform> _waypoints = new List<Transform>();

	#endregion

	#region Private Variables

	private NavMeshAgent _agent;
	private int _currentWaypointIndex;

	#endregion

	public override void Start()
	{
		base.Start();

		_agent = GetComponent<NavMeshAgent>();
	}

	public override void StartAction(Dictionary<string, object> data = null)
	{
		base.StartAction(data);
	}

	public override void DoAction()
	{
		CheckSwitchAction();
		PatrolToWaypoint();
	}

	private void PatrolToWaypoint()
	{
		float distanceFromWaypoint = Vector3.Distance(_waypoints[_currentWaypointIndex].transform.position, transform.position);

		_agent.SetDestination(_waypoints[_currentWaypointIndex].transform.position);

		if (distanceFromWaypoint > _agent.stoppingDistance) { return; }

		_currentWaypointIndex++;

		if (_currentWaypointIndex >= _waypoints.Count) { _currentWaypointIndex = 0; }
	}

	public override void CheckSwitchAction()
	{
		base.CheckSwitchAction();

		if (_aiCues.Ship) { _aiStateMachine.DoChase(); }
	}
}