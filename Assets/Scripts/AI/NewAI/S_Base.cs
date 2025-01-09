using UnityEngine;

public class S_Base : MonoBehaviour
{
	protected AI_Cues _aiCues;

	public virtual void Start()
	{
		_aiCues = GetComponent<AI_Cues>();
	}

	public void Update()
	{
		UpdateSense();
	}

	public virtual void UpdateSense()
	{

	}
}