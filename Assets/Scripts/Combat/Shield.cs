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
    [SerializeField] private GameObject _ball;

    #endregion

    #region Private Variables

    private int _getComponentTries = 4;
    private ShipHealth _shipHealth;

    #endregion

    private void Start()
    {
        _shipHealth = transform.root.GetComponent<ShipHealth>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameObject ball = Instantiate(_ball, transform.position + Vector3.left * 10f , Quaternion.identity);
            ball.GetComponent<AIStateMachine>().BounceOffShield();
            ball.GetComponent<Rigidbody>().AddForce(Vector3.right * 20f, ForceMode.Impulse);
        }

        Debug.DrawRay(transform.position, -transform.up * 50f, Color.red);
        Debug.DrawRay(transform.position, -transform.right * 50f, Color.red);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(LayerMask.LayerToName(collision.gameObject.layer) == "NPC") { CheckForEnemyCollision(collision); return; }

        //If object layer is one of the crash layers, except for NPC
        if (_shipHealth.CrashLayers == (_shipHealth.CrashLayers | (1 << collision.gameObject.layer))) { CheckForSceneCollision(collision); return; }  
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
        Debug.Log("PUSH SHIP");

        Vector3 pushDir = _shipHealth.transform.position - collision.contacts[0].point;

        _shipHealth.Rb.AddForce(pushDir.normalized * _shipHealth.Rb.mass * _shipPushForce, ForceMode.Impulse);
    }

    private void PushEnemy(AIStateMachine aIStateMachine, Collision collision)
    {
        Debug.Log("PUSH: " + aIStateMachine.gameObject.name);

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
