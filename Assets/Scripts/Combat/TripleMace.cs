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

        ApplyForceToMace(0);
        ApplyForceToMace(1);
        ApplyForceToMace(2);
    }

    #endregion

    private void ApplyForceToMace(int index)
    {
        float speedMultiplier = 0;

        switch(index)
        {
            case 0:
                speedMultiplier = 0.6f;
                break;
            case 1:
                speedMultiplier = 0.8f;
                break;
            case 2:
                speedMultiplier = 1;
                break;
        }

        _rbs[index].AddForce(new Vector3(_weapon.CurrentPlayer.MoveDirection.x, _weapon.CurrentPlayer.MoveDirection.y, _weapon.CurrentPlayer.MoveDirection.z) * _moveForce * speedMultiplier);
        _rbs[index].velocity = Vector3.ClampMagnitude(_rbs[index].velocity, _maxMovementSpeed);
    }

    public override void HandleHitParticles(GameObject obj)
    {
        //do nothing
    }
}