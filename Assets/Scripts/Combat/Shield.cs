using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Shield : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private float _pushForce = 20f;
    [SerializeField] private GameObject _ball;

    #endregion

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
        Debug.Log("OnCollisionEnter: " + collision.gameObject.name);
        if (collision.transform.parent.TryGetComponent(out AIStateMachine aIStateMachine)) { PushEnemy(aIStateMachine, collision); }
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

        Vector3 pushDir = Vector3.Reflect(aIStateMachine.transform.position - transform.position, -transform.right);

        Debug.DrawRay(aIStateMachine.transform.position, pushDir * 50f, Color.magenta, 5f);
        //Debug.DrawRay(aIStateMachine.transform.position, pushDir, Color.green, 5f);

        //Vector3 pushDir = contanctPoint;

        rb.AddForce(pushDir.normalized * rb.mass * _pushForce, ForceMode.Impulse);
    }
}
