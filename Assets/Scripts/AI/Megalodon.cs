using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Megalodon : GAgent
{
    private Vector3 _startingRotation;

    private float _currentYTarget;
    private float _yRotVel;
    private float _yRotSmoothTime = 0.3f;

    public override void Start()
    {
        base.Start();

        _startingRotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);

        GetComponent<NavMeshAgent>().updateRotation = false;
        GetComponent<NavMeshAgent>().avoidancePriority = 0;

        //SubGoal s1 = new SubGoal("destroyShip", 1, false);
        //Goals.Add(s1, 1);

        SubGoal s2 = new SubGoal("hasCharged", 1, false);
        Goals.Add(s2, 3);

        //SubGoal s4 = new SubGoal("healthy", 1, false);
        //Goals.Add(s4, 10);

        //SubGoal s3 = new SubGoal("throwRock", 1, false);
        //Goals.Add(s3, 5);
    }

    private void Update()
    {
        Vector3 difVector = CurrentAction ? transform.position - CurrentAction.Target.transform.position : Vector3.zero;

        float yRotationTarget = difVector.x > 0 ? 270f : 90f;

        Quaternion targetRotation = Quaternion.Euler(0f, _currentYTarget, 90f);

        transform.rotation = targetRotation;

        if (_currentYTarget == yRotationTarget) { return; }

        _currentYTarget = yRotationTarget;

        SendMessage("RotatedMessage");

        //StateMachine.Anim.Play("180Turn", 0);
    }

    public void Rotate180()
    {
        //Debug.Log("Rotate180");
        ////float yRotationValue = Mathf.SmoothDamp(transform.eulerAngles.y, _currentYTarget, ref _yRotVel, _yRotSmoothTime);

        //Quaternion targetRotation = Quaternion.Euler(0f, _currentYTarget, 90f);

        //transform.rotation = targetRotation;
    }
}

