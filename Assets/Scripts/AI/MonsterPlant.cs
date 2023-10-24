using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPlant : GAgent
{
    public override void Start()
    {
        base.Start();

        SubGoal s4 = new SubGoal("destroyShip", 1, false);
        Goals.Add(s4, 5);

        //SubGoal s3 = new SubGoal("healthy", 1, false);
        //Goals.Add(s3, 10);
    }
}
