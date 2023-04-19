using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Megalodon : GAgent
{
    #region Editor Fields

    [Header("FX")]
    [SerializeField] private ParticleSystem _rockPickupParticles;

    #endregion

    #region Private Variables

    private float _currentYTarget;

    #endregion

    public override void Start()
    {
        base.Start();

        Damageable.OnDie += HandleBossDied;

        GetComponent<NavMeshAgent>().updateRotation = false;
        GetComponent<NavMeshAgent>().avoidancePriority = 0;
    }

    private void OnDestroy()
    {
        Damageable.OnDie -= HandleBossDied;
    }

    private void HandleBossDied()
    {
        AIHealth aIHealth = Damageable as AIHealth;

        aIHealth.InvokeBossDiedEvent();
    }

    private void Update()
    {
        UpdateRotation();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();

        TryGetNewAction();
    }

    private void TryGetNewAction()
    {
        if(Goals.Count > 0) { return; }

        ////Set of attacks up to 50% health
        //if (_damageable.CurrentHealth > _damageable.MaxHealth * 0.5f)
        //{
        //    int randomAttack = Random.Range(0, 2);

        //    switch (randomAttack)
        //    {
        //        case 0:
        //            SubGoal s2 = new SubGoal("hasCharged", 1, true);
        //            Goals.Add(s2, 3);
        //            break;
        //        case 1:
        //            SubGoal s3 = new SubGoal("throwRock", 1, true);
        //            Goals.Add(s3, 5);
        //            break;

        //    }
        //}
        ////Set of attacks below 50% health
        //else
        //{
            int randomAttack = Random.Range(0, 2);

            switch (randomAttack)
            {
                case 0:
                    SubGoal s6 = new SubGoal("ragdollShip", 1, true);
                    Goals.Add(s6, 3);
                    break;
                case 1:
                    SubGoal s2 = new SubGoal("hasFlappedFins", 1, true);
                    Goals.Add(s2, 3);
                    break;

            }
        //}
    }

    private void UpdateRotation()
    {
        Vector3 difVector = CurrentAction ? transform.position - CurrentAction.Target.transform.position : Vector3.zero;

        float yRotationTarget = difVector.x > 0 ? 270f : 90f;
        if (CurrentAction is MoveInZPlane) { yRotationTarget = 0f; }
        else if (CurrentAction is MegalodonRagdollAttack) { yRotationTarget = 180f; }

        _currentYTarget = Mathf.MoveTowards(_currentYTarget, yRotationTarget, 300f * Time.deltaTime);

        Quaternion targetRotation = Quaternion.Euler(0f, _currentYTarget, 90f);

        transform.rotation = targetRotation;

        if (Mathf.Abs(_currentYTarget - yRotationTarget) < 5f) { return; }

        SendMessage("RotatedMessage");

        switch (yRotationTarget)
        {
            case 90f:
                StateMachine.Anim.Play("180Turn", 0);
                break;
            case 270f:
                StateMachine.Anim.Play("-180Turn", 0);
                break;
            case 0f:
                StateMachine.Anim.Play("90Turn", 0);
                break;
            case 180f:
                StateMachine.Anim.Play("-90Turn", 0);
                break;
        }
    }
    
    public void PickupRockEvent()
    {
        _rockPickupParticles.Play();
    }
}

