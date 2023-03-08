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

    public virtual void Update()
    {
        if (_currentPlayer == null) { return; }

        if (CanUse == false) { return; }

        Rotate();
    }

    #endregion

    public virtual void Rotate()
    {
        if (_currentPlayer.MoveDirection.magnitude == 0) { return; }
        
        _currentAngle = _rotationSpeed * _currentPlayer.MoveDirection.x * Time.deltaTime;
        RotatorTransform.RotateAround(_pivotTransform.position, Vector3.forward, -_currentAngle);
    }
}
