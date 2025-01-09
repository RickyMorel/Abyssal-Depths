using UnityEngine;

public class S_Vision : S_Base
{
	#region Editor Fields

	[SerializeField] private float _visionRange = 10f;

	#endregion

	#region Private Variables

	private float _timePassedSinceLastSawPlayer;
	private float _unseePlayerTime = 0.2f;

	#endregion

	private void Update()
	{
		UpdateSense();
	}

	private void DoRaycast()
	{
		Vector3 raycastDirection = Ship.Instance.transform.position - transform.position;

		if (Physics.Raycast(transform.position, raycastDirection, out RaycastHit hit, _visionRange))
		{
			Debug.DrawLine(transform.position, hit.point, Color.red);
			CheckHitPlayer(hit);
		}
	}

	private void CheckHitPlayer(RaycastHit hit)
	{
		if (!hit.rigidbody) { return; }

		if (hit.rigidbody.GetComponent<Ship>() == null) { return; }

		Debug.Log("PLAYERRRRRRRRRRRRRRRRRRRR!");

		_timePassedSinceLastSawPlayer = 0f;

		_aiCues.SetPlayer(hit.rigidbody.GetComponent<Ship>());
		Debug.Log("Hit Player!");
	}

	public override void UpdateSense()
	{
		_timePassedSinceLastSawPlayer += Time.deltaTime;

		if (_timePassedSinceLastSawPlayer > _unseePlayerTime) { _aiCues.SetPlayer(null); }

		DoRaycast();
	}
}