using UnityEngine;

public class AI_ShootController : MonoBehaviour
{
	#region Editor Fields

	[SerializeField] protected GameObject _projectilePrefab;
	[SerializeField] protected Transform _shootTransform;
	[SerializeField] private int _enemyDamageDataID;
	[SerializeField] private ParticleSystem _hitParticle;

	#endregion

	#region Private Variables

	private AI_StateMachine _aiStateMachine;

	#endregion

	#region Public Properties


	#endregion


	private void Start()
	{
		_aiStateMachine = GetComponent<AI_StateMachine>();
	}

	public void Shoot()
	{
		Transform enemyTransform = Ship.Instance.transform;
		GameObject newProjectile = Instantiate(_projectilePrefab, _shootTransform.position, _shootTransform.rotation);
		newProjectile.transform.LookAt(enemyTransform);
		Projectile projectile = newProjectile.GetComponent<Projectile>();
		projectile.Initialize(tag, transform);
		projectile.AICombatID = _enemyDamageDataID;

		_aiStateMachine.Audio.PlayShootSFX();
	}
}
