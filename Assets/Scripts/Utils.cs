using log4net.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Codice.Client.Common.WebApi.WebApiEndpoints;
using UnityEngine.AI;

public static class Utils
{
    public static Vector3 RandomNavmeshLocation(NavMeshAgent agent, float radius, float minDIstance, float maxDistance, int navMeshMask = NavMesh.AllAreas)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += agent.transform.position;
        NavMeshHit hit;
        Vector3 center = Ship.Instance.transform.position;
        Vector3 finalPosition = Vector3.zero;
        int foundNavmeshMask = NavMesh.AllAreas;
        int crashPreventionCounter = 0;

        //Get current mesh area mask
        agent.SamplePathPosition(navMeshMask, 0f, out NavMeshHit navMeshHit);

        //while current mask != the sampled mask, look again
        while (navMeshHit.mask != foundNavmeshMask)
        {
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, navMeshMask))
            {
                finalPosition = hit.position;
                foundNavmeshMask = hit.mask;

                //If random point is closer than min distance, project point further out
                float remainingMinDistance = Vector3.Distance(center, finalPosition);
                if (remainingMinDistance < minDIstance)
                    if (NavMesh.SamplePosition(finalPosition + (randomDirection.normalized * remainingMinDistance), out hit, maxDistance, navMeshMask))
                    {
                        finalPosition = hit.position;
                        break;
                    }
            }

            crashPreventionCounter++;

            //prevents from looping while infinitly if doesn't find position
            if (crashPreventionCounter > 30) { finalPosition = agent.transform.position; Debug.LogError("Did not find position! Check area mask settings!"); break; }
        }

        return finalPosition;
    }
}
