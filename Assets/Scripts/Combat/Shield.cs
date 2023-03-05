using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Shield : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private float _enemyPushForce = 20f;
    [SerializeField] private float _shipPushForce = 10f;

    #endregion

    #region Private Variables

    private int _getComponentTries = 4;
    private ShipHealth _shipHealth;

    #endregion

    private void Start()
    {
        _shipHealth = transform.root.GetComponent<ShipHealth>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Projectile projectile)) { ReflectProjectile(projectile); }

        if(LayerMask.LayerToName(collision.gameObject.layer) == "NPC") { CheckForEnemyCollision(collision); return; }

        //If object layer is one of the crash layers, except for NPC
        if (_shipHealth.CrashLayers == (_shipHealth.CrashLayers | (1 << collision.gameObject.layer))) { CheckForSceneCollision(collision); return; }  
    }

    private void ReflectProjectile(Projectile projectile)
    {
        projectile.RefelctFromShield(_shipHealth.tag);
    }

    private void CheckForSceneCollision(Collision collision)
    {
        PushShip(collision);
    }

    private void CheckForEnemyCollision(Collision collision)
    {
        //Recursively tries to fetch the root gameobject of the collision
        Transform parentTransform = collision.transform.parent;

        for (int i = 0; i < _getComponentTries; i++)
        {
            if (parentTransform.TryGetComponent(out AIStateMachine aIStateMachine)) { PushEnemy(aIStateMachine, collision); break; }

            parentTransform = parentTransform.parent;
        }
    }

    private void PushShip(Collision collision)
    {
        Vector3 pushDir = _shipHealth.transform.position - collision.contacts[0].point;

        _shipHealth.Rb.AddForce(pushDir.normalized * _shipHealth.Rb.mass * _shipPushForce, ForceMode.Impulse);
    }

    private void PushEnemy(AIStateMachine aIStateMachine, Collision collision)
    {
        aIStateMachine.BounceOffShield();

        StartCoroutine(PushEnemyDelay(aIStateMachine, collision.contacts[0].point));
    }

    private IEnumerator PushEnemyDelay(AIStateMachine aIStateMachine, Vector3 contanctPoint)
    {
        yield return new WaitForEndOfFrame();

        Rigidbody rb = aIStateMachine.GetComponent<Rigidbody>();

        Vector3 pushDir = aIStateMachine.transform.position - contanctPoint;

        rb.AddForce(pushDir.normalized * rb.mass * _enemyPushForce, ForceMode.Impulse);
    }
}
