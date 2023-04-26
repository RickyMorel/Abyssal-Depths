using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : Interactable
{
    #region Editor Fields

    [SerializeField] private float _moveSpeed = 5f;

    #endregion

    #region Private Variables

    private float _timeUsed;

    #endregion

    #region Unity Loops

    public void Update()
    {
        if (CurrentPlayer == null) { return; }

        if (CanUse == false) { return; }

        _timeUsed += Time.deltaTime;

        CheckIfExit();

        MovePlayer();
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        RemoveCurrentPlayer();
    }

    #endregion

    private void CheckIfExit()
    {
        if(CurrentPlayer.MoveDirection.x == 0f) { return; }

        //Prevents player from getting off on the same frame as getting on
        if(_timeUsed < 0.5f) { return; }

        RemoveCurrentPlayer();

        _timeUsed = 0f;
    }

    public void MovePlayer()
    {
        float vertical = CurrentPlayer.MoveDirection.y;

        Vector3 moveDir = new Vector3(0f, vertical * _moveSpeed * Time.deltaTime, 0f);

        CurrentPlayer.transform.position += moveDir;
        CurrentPlayer.transform.position = new Vector3(transform.position.x, CurrentPlayer.transform.position.y, CurrentPlayer.transform.position.z);
        CurrentPlayer.transform.rotation = transform.rotation;
    }
}
