using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleMace : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private Mace[] _maces;
    [SerializeField] private Rigidbody[] _rbs;
    [SerializeField] private float _maceDistance = 8f;

    #endregion

    #region Public Properties

    public Rigidbody[] rbs => _rbs;

    #endregion

    #region Unity Loops

    public void FixedUpdate()
    {
        if(_maces[0].Weapon.CurrentPlayer == null) { return; }

        float yDir = Mathf.RoundToInt(_maces[0].Weapon.CurrentPlayer.MoveDirection.y);

        //Separates maces according to which one is closest to the target position
        if (Vector3.Distance(_rbs[0].transform.position, _rbs[1].transform.position) < _maceDistance)
        {
            if (yDir == 1)
            {
                Vector3 dir = _rbs[1].transform.position - _rbs[0].transform.position;
                ApplyForceToMace(1, dir.normalized);
            }
            else if (yDir == -1)
            {
                Vector3 dir = _rbs[0].transform.position - _rbs[1].transform.position;
                ApplyForceToMace(0, dir.normalized);
            }
        }
    }

    #endregion

    public void ApplyForceToMace(int index, Vector3 moveDirection)
    {
        _rbs[index].AddForce(moveDirection * _maces[index].MoveForce);
        _rbs[index].velocity = Vector3.ClampMagnitude(_rbs[index].velocity, _maces[index].MaxMovementSpeed);
    }
}