using System.Collections;
using UnityEngine;

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
    private float _timeSincePushEnemy;

    #endregion

    private void Start()
    {
        _shipHealth = transform.root.GetComponent<ShipHealth>();
    }

    private void Update()
    {
        _timeSincePushEnemy += Time.deltaTime;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Projectile projectile)) { ReflectProjectile(projectile); }

        if(LayerMask.LayerToName(collision.gameObject.layer) == "NPC") { CheckForEnemyCollision(collision); return; }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //If object layer is one of the crash layers, except for NPC
        if (_shipHealth.CrashLayers == (_shipHealth.CrashLayers | (1 << collision.gameObject.layer))) { CheckForSceneCollision(collision); return; }
    }

    private void ReflectProjectile(Projectile projectile)
    {
        projectile.RefelctFromShield(_shipHealth.tag);

        _pushParticles.Play();
    }

    private void CheckForSceneCollision(Collision collision)
    {
        if(_timeSincePushEnemy < 1f) { return; }

        PushShip(collision);
    }

    private void CheckForEnemyCollision(Collision collision)
    {
        Ship.Instance.ShipHealth.SetInvunerableToCrash();

        //Recursively tries to fetch the root gameobject of the collision
        Transform parentTransform = collision.transform.parent;

        for (int i = 0; i < _getComponentTries; i++)
        {
            if (parentTransform.TryGetComponent(out AIStateMachine aIStateMachine)) { PushEnemy(aIStateMachine, collision); break; }

            parentTransform = parentTransform.parent;

            if (parentTransform == null) { break; }
        }
    }

    private void PushShip(Collision collision)
    {
        Vector3 pushDir = _shipHealth.transform.position - collision.contacts[0].point;

        _shipHealth.Rb.AddForce(pushDir.normalized * _shipHealth.Rb.mass * _shipPushForce, ForceMode.Impulse);

        _pushParticles.Play();
    }

    private void PushEnemy(AIStateMachine aIStateMachine, Collision collision)
    {
        aIStateMachine.BounceOffShield();

        StartCoroutine(PushEnemyDelay(aIStateMachine, collision.contacts[0].point));
    }

    private IEnumerator PushEnemyDelay(AIStateMachine aIStateMachine, Vector3 contanctPoint)
    {
        yield return new WaitForEndOfFrame();

        _timeSincePushEnemy = 0f;

        _pushParticles.Play();

        //Makes ship invunerable so ship doesn't recive damage when hitting enemies with the shield
        Ship.Instance.ShipHealth.SetInvunerableToCrash(1f);

        Rigidbody rb = aIStateMachine.GetComponent<Rigidbody>();

        Vector3 pushDir = aIStateMachine.transform.position - contanctPoint;

        Ship.Instance.Rb.AddForce(-pushDir.normalized * rb.mass, ForceMode.Impulse);

        if(rb.velocity.magnitude < _enemyPushForce * 0.8f)
        {
            rb.AddForce(pushDir.normalized * rb.mass * _enemyPushForce, ForceMode.Impulse);
        }
    }
}
