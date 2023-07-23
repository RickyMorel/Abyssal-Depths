using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationalInteractable : Upgradable
{
    #region Editor Fields

    [Header("Rotator Parameters")]
    [SerializeField] protected float _rotationSpeed = 20f;
    [SerializeField] protected Transform _pivotTransform;

    #endregion

    #region Private Variables

    private RotationalInteractableHumble _rotationalHumble;
    protected float _currentAngle;
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

    public override void Awake()
    {
        base.Awake();

        _humble = new RotationalInteractableHumble(IsAIOnlyInteractable);
        _rotationalHumble = _humble as RotationalInteractableHumble;
    }

    public override void Start()
    {
        base.Start();

        _radius = _rotationalHumble.CalculateRadius(_pivotTransform.position, RotatorTransform.position);
    }

    public virtual void Update()
    {
        if (CurrentPlayer == null) { return; }

        if (CanUse == false) { return; }

        if (UsesWASDRotation()) { RotateWASD(); }
        else { Rotate(); }
    }

    #endregion

    private bool UsesWASDRotation()
    {
        return this is ShieldWheel;
    }

    public virtual void RotateWASD()
    {
        float horizontal = CurrentPlayer.MoveDirection.x;
        float vertical = CurrentPlayer.MoveDirection.y;

        _currentAngle = _rotationalHumble.CalculateCurrentAngle(horizontal, vertical, _currentAngle, _rotationSpeed);

        SetRotationWASD();
    }

    private void SetRotationWASD()
    {
        _rotationalHumble.CalculateRotationWASD(_pivotTransform.position, RotatorTransform.position, _radius, _currentAngle, out Vector3 finalPos, out Quaternion finalRotation);

        RotatorTransform.position = finalPos;
        RotatorTransform.rotation = finalRotation;
    }

    public virtual void Rotate()
    {
        if (CurrentPlayer.MoveDirection.magnitude == 0) { return; }

        _currentAngle = _rotationSpeed * CurrentPlayer.MoveDirection.x * Time.deltaTime;
        RotatorTransform.RotateAround(_pivotTransform.position, Vector3.forward, -_currentAngle);
    }

    public void SetRotatorTransform(Transform newTransform)
    {
        RotatorTransform = newTransform;
    }
}
