using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveInZPlane : GAction
{
    #region Editor Fields

    [Header("Action Specific Variables")]
    [SerializeField] private float _swimAwaySpeed = 50f;
    [SerializeField] private float _zDistance = 60f;

    #endregion

    #region Private Variables

    private GameObject _currentRunAwayObj;
    private bool _goToZPos;

    #endregion

    public override bool PrePerform()
    {
        GAgent.StateMachine.CanMove = false;

        GameObject newTargetObj = new GameObject();
        newTargetObj.transform.position = transform.position + Vector3.forward * _zDistance;
        newTargetObj.name = "ZPosTarget";

        _currentRunAwayObj = newTargetObj;

        Target = newTargetObj;

        if (Target == null) { return false; }

        _goToZPos = true;

        return true;
    }

    private void Update()
    {
        if(_goToZPos == false) { return; }

        GetToZPos();
    }

    //Slowly move in the z axis
    private void GetToZPos()
    {
        if(_currentRunAwayObj == null) { return; }

        var step = _swimAwaySpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _currentRunAwayObj.transform.position, step);
    }

    public override bool Perform()
    {
        return true;
    }

    public override bool PostPeform()
    {
        if(_currentRunAwayObj != null) { Destroy(_currentRunAwayObj); }

        //Aligns npc with ship on x and y axis
        transform.position = new Vector3(Ship.Instance.transform.position.x, Ship.Instance.transform.position.y, transform.position.z);

        _goToZPos = false;

        return true;
    }
}
