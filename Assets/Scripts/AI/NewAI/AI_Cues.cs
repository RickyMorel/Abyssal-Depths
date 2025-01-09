using UnityEngine;

public class AI_Cues : MonoBehaviour
{
	#region Editor Fields

	[SerializeField] private Ship _ship;
	[SerializeField] private Vector3 _playerLastSeenPosition;

	#endregion

	#region Private Variables


	#endregion

	#region Public Properties

	public Ship Ship => _ship;
	public Vector3 PlayerLastSeenPosition => _playerLastSeenPosition;

	#endregion

	public void SetPlayer(Ship ship)
	{
		_ship = ship;
	}

	public void SetPlayerLastSeenSpot(Vector3 seenPosition)
	{
		_playerLastSeenPosition = seenPosition;
	}
}