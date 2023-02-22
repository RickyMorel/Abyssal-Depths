using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Shield : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private float _pushForce = 20f;

    #endregion

    public void OnTriggerEnter(Collider other)
    {
       // if (other.GetComponent<Projectile>() != null) { Destroy(other.gameObject); }

        if (other.TryGetComponent(out AIHealth aIHealth)) { PushEnemy(aIHealth); }
    }

    private void PushEnemy(AIHealth aIHealth)
    {
        Debug.Log("PUSH: " + aIHealth.gameObject.name);
        Rigidbody rb = aIHealth.GetComponent<Rigidbody>();
        NavMeshAgent agent = aIHealth.GetComponent<NavMeshAgent>();

        rb.isKinematic = false;
        rb.useGravity = true;
        agent.enabled = false;

        Vector3 pushDir = aIHealth.transform.position - transform.position;

        rb.AddForce(pushDir.normalized * rb.mass * _pushForce, ForceMode.Impulse);
    }
}
