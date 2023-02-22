using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationalInteractable : Upgradable
{
    #region Editor Fields

    [Header("Rotator Parameters")]
    [SerializeField] private float _rotationSpeed = 20f;
    [SerializeField] private Transform _pivotTransform;

    #endregion

    #region Private Variables

    private float _currentAngle = 0;
    private float _radius;

    #endregion

    #region Public Properties

    public Transform RotatorTransform;
    public Transform PivotTransform => _pivotTransform;

    #endregion

    #region Getters & Setters

    public float CurrentAngle { get { return _currentAngle; } set { _currentAngle = value; } }
    public float RotationSpeed { get { return _rotationSpeed; } set { _rotationSpeed = value; } }

    #endregion

    #region Unity Loops

    public override void Start()
    {
        base.Start();

        _radius = Vector3.Distance(_pivotTransform.position, RotatorTransform.position);
    }

    public virtual void Update()
    {
        if (_currentPlayer == null) { return; }

        if (CanUse == false) { return; }

        if (this is Shield) { RotateWASD(); }
        else { Rotate(); }
    }

    #endregion

    public virtual void RotateWASD()
    {
        if (_currentPlayer.MoveDirection.magnitude == 0) { return; }

        float horizontal = _currentPlayer.MoveDirection.x;
        float vertical = _currentPlayer.MoveDirection.y;

        if (Mathf.Abs(horizontal) > 0f || Mathf.Abs(vertical) > 0f)
        {
            float targetAngle = Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg;
            _currentAngle = Mathf.MoveTowardsAngle(_currentAngle, targetAngle, _rotationSpeed * Time.deltaTime);
        }

        float x = _pivotTransform.position.x + _radius * Mathf.Cos(_currentAngle * Mathf.Deg2Rad);
        float y = _pivotTransform.position.y + _radius * Mathf.Sin(_currentAngle * Mathf.Deg2Rad);
        float z = RotatorTransform.position.z;

        RotatorTransform.position = new Vector3(x, y, z);
        RotatorTransform.rotation = Quaternion.LookRotation(Vector3.forward, _pivotTransform.position - RotatorTransform.position);
    }

    public virtual void Rotate()
    {
        if (_currentPlayer.MoveDirection.magnitude == 0) { return; }

        _currentAngle = _rotationSpeed * _currentPlayer.MoveDirection.x * Time.deltaTime;
        RotatorTransform.RotateAround(_pivotTransform.position, Vector3.forward, -_currentAngle);
    }
}
