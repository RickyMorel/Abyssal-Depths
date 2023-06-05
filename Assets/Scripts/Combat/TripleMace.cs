using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleMace : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private Mace[] _maces;
    [SerializeField] private Rigidbody[] _rbs;
    [SerializeField] private Transform _moveLimits;

    #endregion

    #region Public Properties

    public Rigidbody[] rbs => _rbs;

    #endregion

    #region Unity Loops

    public void FixedUpdate()
    {
        Vector3 moveDirection;

        if (_maces[0].Weapon.CurrentPlayer == null)
        {
            moveDirection = Vector3.zero;
        }
        else
        {
            moveDirection = _maces[0].Weapon.CurrentPlayer.MoveDirection;
        }

        ApplyForceToMace(0, moveDirection);
        ApplyForceToMace(1, moveDirection);
        ApplyForceToMace(2, moveDirection);
    }

    #endregion

    public void ApplyForceToMace(int index, Vector3 moveDirection)
    {
        if (_rbs[index].transform.position.x >= _moveLimits.transform.position.x && moveDirection.x > 0)
        {
            _rbs[index].velocity = Vector3.zero;
            return;
        }
        else if (_rbs[index].transform.position.x >= _moveLimits.transform.position.x)
        {
            _rbs[index].velocity = Vector3.zero;
        }

        _rbs[index].AddForce(moveDirection * _maces[index].MoveForce);
        _rbs[index].velocity = Vector3.ClampMagnitude(_rbs[index].velocity, _maces[index].MaxMovementSpeed);
    }
}