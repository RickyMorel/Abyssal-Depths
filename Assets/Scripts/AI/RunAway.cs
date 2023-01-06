using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunAway : GAction
{
    #region Editor Fields

    [Header("Action Specific Variables")]
    [SerializeField] private float _minDistance = 30f;
    [SerializeField] private float _maxDistance = 60f;

    #endregion

    #region Private Variables

    private GameObject _currentRunAwayObj;

    #endregion

    public override bool PrePerform()
    {
        Vector3 destination = RandomNavmeshLocation(_maxDistance);

        GameObject newTargetObj = new GameObject();
        newTargetObj.transform.position = destination;
        newTargetObj.name = "RunAwayTargetObj";

        _currentRunAwayObj = newTargetObj;

        Target = newTargetObj;

        if (Target == null) { return false; }

        return true;
    }

    public override bool PostPeform()
    {
        if(_currentRunAwayObj != null) { Destroy(_currentRunAwayObj); }

        return true;
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        int foundNavmeshMask = NavMesh.AllAreas;
        int crashPreventionCounter = 0;

        //Get current mesh area mask
        Agent.SamplePathPosition(NavMesh.AllAreas, 0f, out NavMeshHit navMeshHit);

        //while current mask != the sampled mask, look again
        while(navMeshHit.mask != foundNavmeshMask)
        {
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
            {
                finalPosition = hit.position;
                foundNavmeshMask = hit.mask;

                //If random point is closer than min distance, project point further out
                float remainingMinDistance = Vector3.Distance(transform.position, finalPosition);
                if (remainingMinDistance < _minDistance)
                    if (NavMesh.SamplePosition(finalPosition + (randomDirection.normalized * remainingMinDistance), out hit, _maxDistance, NavMesh.AllAreas))
                    {
                        finalPosition = hit.position;
                        Debug.Log("Got Updated Min Random pos");
                    }
            }

            crashPreventionCounter++;

            //prevents from looping while infinitly if doesn't find position
            if(crashPreventionCounter > 30) { finalPosition = transform.position; break; }
        }

        return finalPosition;
    }
}
