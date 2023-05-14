using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleMace : MeleeWeapon
{
    #region Editor Fields

    [SerializeField] private GameObject[] _maceHead;
    [SerializeField] private Rigidbody[] _rbs;
    [SerializeField] private Transform _moveLimits;
    [SerializeField] private Booster _booster;

    #endregion

    #region Public Properties

    public Rigidbody[] rbs => _rbs;

    #endregion

    #region Unity Loops

    public override void Awake()
    {
        Booster.OnBoostUpdated += ApplyBoostToMace;
    }

    public override void OnDestroy()
    {
        Booster.OnBoostUpdated -= ApplyBoostToMace;
    }

    public override void FixedUpdate()
    {
        Vector3 moveDirection;

        if (_weapon.CurrentPlayer == null)
        {
            moveDirection = Vector3.zero;
        }
        else
        {
            moveDirection = _weapon.CurrentPlayer.MoveDirection;
        }

        ApplyForceToMace(0, moveDirection);
        ApplyForceToMace(1, moveDirection);
        ApplyForceToMace(2, moveDirection);

        ApplyBoostToMace(_booster.IsBoosting);
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

        _rbs[index].AddForce(moveDirection * _moveForce);
        _rbs[index].velocity = Vector3.ClampMagnitude(_rbs[index].velocity, _maxMovementSpeed);
    }

    private void ApplyBoostToMace(bool isBoosting)
    {
        if (!isBoosting) { return; }
        Debug.Log(_booster);
        _rbs[0].AddForce(_booster.RotatorTransform.transform.up * _booster.Acceleration * _booster.RB.mass);

        _rbs[0].velocity = Vector3.ClampMagnitude(_booster.RB.velocity, Ship.Instance.TopSpeed);
    }

    public override void HandleHitParticles(GameObject obj)
    {
        //do nothing
    }
}