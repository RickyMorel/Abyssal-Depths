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

    private void OnTriggerStay(Collider collider)
    {
        CheckForSceneCollision(collider);
    }

    private void CheckForSceneCollision(Collider collider)
    {
        if (_timeSincePushEnemy < 1f) { return; }

        //Don't push ship if hits shield
        if(collider.gameObject.GetComponent<Shield>() != null) { Debug.Log("Hit Shield!"); return; }

        PushShip(collider);
    }

    private void PushShip(Collider collider)
    {
        StartCoroutine(PushShipDelay(collider.gameObject.transform.position));
    }

    private IEnumerator PushShipDelay(Vector3 targetPos)
    {
        yield return new WaitForEndOfFrame();

        _timeSincePushEnemy = 0f;

       // _pushParticles.Play();

        Vector3 pushDir = transform.position - targetPos;

        Ship.Instance.Rb.AddForce(-pushDir.normalized * _pushVelocity, ForceMode.VelocityChange);

        ShipCamera.Instance.ShakeCamera(2f, 50f, 0.2f);

        Debug.Log("PushShip!");
    }
}
