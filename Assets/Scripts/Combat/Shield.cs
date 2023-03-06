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
    [SerializeField] private ParticleSystem _pushParticles;

    #endregion

    #region Private Variables

    private int _getComponentTries = 4;
    private ShipHealth _shipHealth;

    #endregion

    private void Start()
    {
        _shipHealth = transform.root.GetComponent<ShipHealth>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Projectile projectile)) { ReflectProjectile(projectile); }

        if(LayerMask.LayerToName(other.gameObject.layer) == "NPC") { CheckForEnemyCollision(other); return; }

        //If object layer is one of the crash layers, except for NPC
        if (_shipHealth.CrashLayers == (_shipHealth.CrashLayers | (1 << other.gameObject.layer))) { CheckForSceneCollision(other); return; }  
    }

    private void ReflectProjectile(Projectile projectile)
    {
        projectile.RefelctFromShield(_shipHealth.tag);
    }

    private void CheckForSceneCollision(Collider other)
    {
        //PushShip(other);
    }

    private void CheckForEnemyCollision(Collider other)
    {
        Ship.Instance.ShipHealth.SetInvunerableToCrash();

        //Recursively tries to fetch the root gameobject of the collision
        Transform parentTransform = other.transform.parent;

        for (int i = 0; i < _getComponentTries; i++)
        {
            if (parentTransform.TryGetComponent(out AIStateMachine aIStateMachine)) { PushEnemy(aIStateMachine, other); break; }

            parentTransform = parentTransform.parent;
        }
    }

    private void PushShip(Collider other)
    {
        Vector3 pushDir = _shipHealth.transform.position - other.ClosestPointOnBounds(transform.position); ;

        _shipHealth.Rb.AddForce(pushDir.normalized * _shipHealth.Rb.mass * _shipPushForce, ForceMode.Impulse);

        _pushParticles.Play();
    }

    private void PushEnemy(AIStateMachine aIStateMachine, Collider other)
    {
        aIStateMachine.BounceOffShield();

        StartCoroutine(PushEnemyDelay(aIStateMachine, other.ClosestPointOnBounds(aIStateMachine.transform.position)));
    }

    private IEnumerator PushEnemyDelay(AIStateMachine aIStateMachine, Vector3 contanctPoint)
    {
        yield return new WaitForEndOfFrame();

        Rigidbody rb = aIStateMachine.GetComponent<Rigidbody>();

        Vector3 pushDir = aIStateMachine.transform.position - contanctPoint;

        rb.AddForce(pushDir.normalized * rb.mass * _enemyPushForce, ForceMode.Impulse);

        _pushParticles.Play();

        Ship.Instance.Rb.AddForce(-pushDir.normalized * rb.mass, ForceMode.Impulse);
    }
}
