using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyPushAttackCollider : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private float _pushVelocity = 10f;
    [SerializeField] private ParticleSystem _pushParticles;

    #endregion

    #region Private Variables

    private float _timeSincePushEnemy;
    private Collider[] _colliders;

    #endregion

    private void Start()
    {
        _colliders = GetComponents<Collider>();
    }

    private void Update()
    {
        _timeSincePushEnemy += Time.deltaTime;
    }

    private void EnableColliders(bool isEnabled)
    {
        foreach (Collider collider in _colliders)
        {
            collider.enabled = isEnabled;
        }
    }

    public void PushAttack()
    {
        EnableColliders(true);

        StartCoroutine(DisablePushAttack());
    }

    private IEnumerator DisablePushAttack()
    {
        yield return new WaitForSeconds(0.5f);

        EnableColliders(false);
    }

    private void OnCollisionStay(Collision collision)
    {
        CheckForSceneCollision(collision);   
    }

    private void CheckForSceneCollision(Collision collision)
    {
        if (_timeSincePushEnemy < 1f) { return; }

        PushShip(collision);
    }

    private void PushShip(Collision collision)
    {
        StartCoroutine(PushShipDelay(collision.contacts[0].point));
    }

    private IEnumerator PushShipDelay(Vector3 contanctPoint)
    {
        yield return new WaitForEndOfFrame();

        _timeSincePushEnemy = 0f;

       // _pushParticles.Play();

        Vector3 pushDir = transform.position - contanctPoint;

        Ship.Instance.Rb.AddForce(-pushDir.normalized * _pushVelocity, ForceMode.VelocityChange);

        ShipCamera.Instance.ShakeCamera(2f, 50f, 0.2f);
    }
}
