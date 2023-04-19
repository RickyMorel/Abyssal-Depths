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
        yield return new WaitForSeconds(3f);

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
        if(collider.gameObject.GetComponent<Shield>() != null) { return; }

        if (collider.gameObject.tag == "MainShip") { PushShip(); }
    }

    private void PushShip()
    {
        StartCoroutine(PushShipDelay());
    }

    private IEnumerator PushShipDelay()
    {
        yield return new WaitForEndOfFrame();

        _timeSincePushEnemy = 0f;

        Vector3 pushDir = transform.position - Ship.Instance.transform.position;

        Ship.Instance.Rb.AddForce(-pushDir.normalized * _pushVelocity, ForceMode.VelocityChange);

        ShipCamera.Instance.NormalShake();
    }
}
