using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Shark : GAgent
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

        GetComponent<NavMeshAgent>().updateRotation = false;
        GetComponent<NavMeshAgent>().avoidancePriority = 0;

        SubGoal s3 = new SubGoal("throwRock", 1, false);
        Goals.Add(s3, 5);
    }

    //private void Update()
    //{
    //    UpdateRotation();
    //}

    //public override void LateUpdate()
    //{
    //    base.LateUpdate();

    //    TryGetNewAction();
    //}

    private void TryGetNewAction()
    {
        if (!NeedsNewGoal()) { return; }

        Goals.Clear();

        //Set of attacks up to 50% health
        if (_damageable.CurrentHealth > _damageable.MaxHealth * 0.5f)
        {
            int randomAttack = Random.Range(0, 3);

            switch (randomAttack)
            {
                case 0:
                    SubGoal s2 = new SubGoal("hasCharged", 1, true);
                    Goals.Add(s2, 3);
                    break;
                case 1 or 2:
                    SubGoal s3 = new SubGoal("throwRock", 1, true);
                    Goals.Add(s3, 5);
                    break;

            }
        }
    }

    private void UpdateRotation()
    {
        if(CurrentAction == null || CurrentAction.Target == null) { return; }

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

