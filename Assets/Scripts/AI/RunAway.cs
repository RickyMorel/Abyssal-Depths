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
    private int _navMeshMask = NavMesh.AllAreas;
    private Vector3 _startPosition;
    private float _timeSinceDoAction;

    #endregion

    public override void Start()
    {
        base.Start();

        _startPosition = transform.position;   
    }

    public override bool PrePerform()
    {
        Vector3 destination = RandomNavmeshLocation(_maxDistance);
        _navMeshMask = Agent.areaMask;

        GameObject newTargetObj = new GameObject();
        newTargetObj.transform.position = destination;
        newTargetObj.name = "RunAwayTargetObj";

        _currentRunAwayObj = newTargetObj;

        Target = newTargetObj;

        _timeSinceDoAction = 0f;

        if (Target == null) { return false; }

        return true;
    }

    private void Update()
    {
        if(GAgent.CurrentAction != this) { return; }

        _timeSinceDoAction += Time.deltaTime;

        if(_timeSinceDoAction < 8f) { return; }

        //transform.position = _startPosition;

        GAgent.CancelPreviousActions();
    }

    public override bool Perform()
    {
        return true;
    }

    public override bool PostPeform()
    {
        if(_currentRunAwayObj != null) { Destroy(_currentRunAwayObj); }

        Beliefs.RemoveState("hurt");

        return true;
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 center = Ship.Instance.transform.position;
        Vector3 finalPosition = Vector3.zero;
        int foundNavmeshMask = NavMesh.AllAreas;
        int crashPreventionCounter = 0;

        //Get current mesh area mask
        Agent.SamplePathPosition(_navMeshMask, 0f, out NavMeshHit navMeshHit);

        //while current mask != the sampled mask, look again
        while(navMeshHit.mask != foundNavmeshMask)
        {
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, _navMeshMask))
            {
                finalPosition = hit.position;
                foundNavmeshMask = hit.mask;

                //If random point is closer than min distance, project point further out
                float remainingMinDistance = Vector3.Distance(center, finalPosition);
                if (remainingMinDistance < _minDistance)
                    if (NavMesh.SamplePosition(finalPosition + (randomDirection.normalized * remainingMinDistance), out hit, _maxDistance, _navMeshMask))
                    {
                        finalPosition = hit.position;
                        break;
                    }
            }

            crashPreventionCounter++;

            //prevents from looping while infinitly if doesn't find position
            if(crashPreventionCounter > 30) { finalPosition = transform.position; Debug.LogError("Did not find position! Check area mask settings!"); break; }
        }

        return finalPosition;
    }
}
