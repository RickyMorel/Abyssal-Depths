using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleMace : MeleeWeapon
{
    #region Editor Fields

    [SerializeField] private GameObject[] _maceHead;
    [SerializeField] private Rigidbody[] _rbs;

    #endregion

    #region Public Properties

    public Rigidbody[] rbs => _rbs;

    #endregion

    #region Unity Loops

    public override void Awake()
    {
        //do nothing
    }

    public override void OnDestroy()
    {
        //do nothing
    }

    public override void OnEnable()
    {
        _maceHead[0].transform.parent = null;
        _maceHead[1].transform.parent = null;
        _maceHead[2].transform.parent = null;
    }

    public override void OnDisable()
    {
        if (_parentTransform == null) { return; }

        _maceHead[0].transform.parent = _parentTransform;
        _maceHead[1].transform.parent = _parentTransform;
        _maceHead[2].transform.parent = _parentTransform;
    }

    public override void FixedUpdate()
    {
        if (_weapon.CurrentPlayer == null) { return; }
        if (_weapon.CanUse == false) { return; }

        ApplyForceToMace(0, _weapon.CurrentPlayer.MoveDirection);
        ApplyForceToMace(1, _weapon.CurrentPlayer.MoveDirection);
        ApplyForceToMace(2, _weapon.CurrentPlayer.MoveDirection);
    }

    #endregion

    public void ApplyForceToMace(int index, Vector3 moveDirection)
    {
        _rbs[index].AddForce(moveDirection * _moveForce);
        _rbs[index].velocity = Vector3.ClampMagnitude(_rbs[index].velocity, _maxMovementSpeed);
    }

    public override void HandleHitParticles(GameObject obj)
    {
        //do nothing
    }
}