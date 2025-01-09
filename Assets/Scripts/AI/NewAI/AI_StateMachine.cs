using UnityEngine;
using UnityEngine.AI;

public class AI_StateMachine : MonoBehaviour
{
	#region Editor Fields

	[SerializeField] private A_Base[] _actions;

	#endregion

	#region Private Variables

	private A_Base _currentAction;
	private AI_Cues _aiCues;
	private AIAudio _audio;
	private Animator _anim;
	private NavMeshAgent _agent;

	private float gizmoTimer = 0.0f;

	#endregion

	#region Public Properties

	public Animator Anim => _anim;
	public AIAudio Audio => _audio;

	#endregion

	void Start()
	{
		_aiCues = GetComponent<AI_Cues>();
		_anim = GetComponent<Animator>();
		_agent = GetComponent<NavMeshAgent>();
		_audio = GetComponent<AIAudio>();

		DoNewAction(_actions[0]);
	}

	void Update()
	{
		Animate();

		if (!_currentAction) { return; }

		_currentAction.DoAction();

		if (gizmoTimer > 0)
		{
			gizmoTimer -= Time.deltaTime;
		}
	}

	private void Animate()
	{
		_anim.SetFloat("Moving", _agent.velocity.magnitude / _agent.speed);
	}

	private void DoNewAction(A_Base actionToDo)
	{
		actionToDo.StartAction();

		_currentAction = actionToDo;
	}

	public void DoPatrol() { DoNewAction(_actions[0]); }
	public void DoChase() { DoNewAction(_actions[1]); }
	public void DoSearch() { DoNewAction(_actions[2]); }
	public void DoGoToPlayerLastSeenSpot()
	{
		Vector3 lastSeenSpot = FindObjectOfType<Ship>().transform.position;

		_aiCues.SetPlayerLastSeenSpot(lastSeenSpot);

		DoNewAction(_actions[0]);

		gizmoTimer = 2f;
	}

	void OnDrawGizmos()
	{
		if (gizmoTimer <= 0.0f) { return; }

		Vector3 lastSeenSpot = _aiCues.PlayerLastSeenPosition;

		Gizmos.color = Color.red;
		Gizmos.DrawSphere(lastSeenSpot, 0.5f);
	}
}