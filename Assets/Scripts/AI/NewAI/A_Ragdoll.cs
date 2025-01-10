using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class A_Ragdoll : A_Base
{
	#region Editor Fields

	[SerializeField] private Transform _headTrasform;

	#endregion
	#region Private Variables

	private Rigidbody _rb;
	private NavMeshAgent _agent;
	private bool _isKinematicInitialState;
	private bool _useGravityInitialState;
	private bool _agentEnabledInitialState;
	private GameObject _stunnedParticleInstance;
	private ParticleSystem _bubbleParticles;
	private Collider _mainCollider;

	#endregion

	public override void Start()
	{
		base.Start();

		_bubbleParticles = Instantiate(GameAssetsManager.Instance.RagdollBubbleParticles, transform).GetComponent<ParticleSystem>();
		_agent = GetComponent<NavMeshAgent>();
		_rb = GetComponent<Rigidbody>();
		_mainCollider = GetComponent<Collider>();
	}

	public override void StartAction(Dictionary<string, object> data = null)
	{
		base.StartAction(data);

		EnableRagdoll(true);

		Vector3 pushDir = (Vector3)data["pushDir"];
		float pushForce = (float)data["pushForce"];
		StartCoroutine(SetBouncingOffShieldCoroutine(pushDir, pushForce));
	}

	public override void DoAction()
	{
		CheckSwitchAction();
	}

	public override void CheckSwitchAction()
	{
		base.CheckSwitchAction();
	}

	public void EnableRagdoll(bool isEnabled)
	{
		if (isEnabled)
		{
			_rb.isKinematic = false;
			_rb.useGravity = true;
			_agent.enabled = false;
			_mainCollider.isTrigger = false;
			_bubbleParticles.Play();
			EnableStunFX(true);
		}
		else
		{
			_rb.isKinematic = _isKinematicInitialState;
			_rb.useGravity = _useGravityInitialState;
			_agent.enabled = _agentEnabledInitialState;
			_mainCollider.isTrigger = true;
			_bubbleParticles.Stop();
			EnableStunFX(false);
		}
	}

	public void EnableStunFX(bool enable)
	{
		if (enable)
		{
			//Play initial hit particles
			GameObject stunHitParticles = Instantiate(GameAssetsManager.Instance.StunnedParticles[0], _headTrasform.position, _headTrasform.rotation);

			//Play looping stun particles
			_stunnedParticleInstance = Instantiate(GameAssetsManager.Instance.StunnedParticles[1], _headTrasform);
			_stunnedParticleInstance.transform.localScale = _headTrasform.localScale;
		}
		else
		{
			_stunnedParticleInstance.GetComponent<ParticleSystem>().Stop();
		}
	}

	public IEnumerator SetBouncingOffShieldCoroutine(Vector3 pushDir, float pushForce)
	{
		yield return new WaitForEndOfFrame();

		if (_rb.velocity.magnitude < pushForce * 0.8f)
		{
			_rb.AddForce(pushDir.normalized * _rb.mass * pushForce, ForceMode.Impulse);
		}

		yield return new WaitForSeconds(1f);

		while (!IsOnGround(out NavMeshHit groundPos))
		{
			yield return null;
		}

		IsOnGround(out NavMeshHit groundPosConfirmed);

		transform.position = groundPosConfirmed.position;
		transform.rotation = Quaternion.identity;

		StopAllCoroutines();

		EnableRagdoll(false);

		_aiStateMachine.DoPatrol();
	}

	private bool IsOnGround(out NavMeshHit groundPos)
	{
		NavMeshHit hit;

		bool isOnNavMesh = NavMesh.SamplePosition(transform.position, out hit, 5f, _agent.areaMask);

		groundPos = hit;

		return isOnNavMesh;
	}
}
