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

    [Header("Optional Variables")]
    [SerializeField] private Transform[] _runAwayPositions;

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
        _navMeshMask = Agent.areaMask;

        GameObject newTargetObj = new GameObject();
        newTargetObj.transform.position = GetPosition();
        newTargetObj.name = "RunAwayTargetObj";

        _currentRunAwayObj = newTargetObj;

        Target = newTargetObj;

        _timeSinceDoAction = 0f;

        GAgent.StateMachine.AICombat.DestroyPrevHeldProjectile();

        if (Target == null) { return false; }

        return true;
    }

    private Vector3 GetPosition()
    {
        if(_runAwayPositions.Length < 1) { return Utils.RandomNavmeshLocation(Agent, _minDistance, _maxDistance, _navMeshMask); }

        Transform farthestTransform = _runAwayPositions[0];

        foreach (var positionTransform in _runAwayPositions)
        {
            if(Vector3.Distance(Ship.Instance.transform.position, positionTransform.transform.position) >
                Vector3.Distance(Ship.Instance.transform.position, farthestTransform.transform.position))
            {
                farthestTransform = positionTransform;
            }
        }

        return farthestTransform.position;
    }

    private void Update()
    {
        if(GAgent.CurrentAction != this) { return; }

        _timeSinceDoAction += Time.deltaTime;

        if(_timeSinceDoAction < 8f) { return; }

        GAgent.CancelPreviousActions();
    }

    public override bool Perform()
    {
        base.Perform();

        return true;
    }

    public override bool PostPeform()
    {
        if(_currentRunAwayObj != null) { Destroy(_currentRunAwayObj); }

        Beliefs.RemoveState("hurt");

        return true;
    }
}
