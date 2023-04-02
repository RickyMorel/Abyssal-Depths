using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Megalodon : GAgent
{
    public override void Start()
    {
        base.Start();

        //GetComponent<NavMeshAgent>().updateRotation = false;

        //SubGoal s1 = new SubGoal("destroyShip", 1, false);
        //Goals.Add(s1, 1);

        SubGoal s2 = new SubGoal("hasCharged", 1, false);
        Goals.Add(s2, 3);

        //SubGoal s4 = new SubGoal("healthy", 1, false);
        //Goals.Add(s4, 10);

        //SubGoal s3 = new SubGoal("throwRock", 1, false);
        //Goals.Add(s3, 5);
    }
}

