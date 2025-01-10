using System.Collections.Generic;
using UnityEngine;

public class A_Base : MonoBehaviour
{
	protected AI_Cues _aiCues;
	protected AI_StateMachine _aiStateMachine;

	public virtual void Start()
	{
		_aiCues = GetComponent<AI_Cues>();
		_aiStateMachine = GetComponent<AI_StateMachine>();
	}

	public virtual void StartAction(Dictionary<string, object> data = null)
	{

	}

	public virtual void DoAction()
	{

	}

	public virtual void CheckSwitchAction()
	{

	}
}