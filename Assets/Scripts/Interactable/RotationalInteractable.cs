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
    private bool _rotateOnUpdate = true;

    #endregion

    #region Public Properties

    public Transform RotatorTransform;
    public Transform PivotTransform => _pivotTransform;

    #endregion

    #region Getters & Setters

    public float CurrentAngle { get { return _currentAngle; } set { _currentAngle = value; } }
    public float RotationSpeed { get { return _rotationSpeed; } set { _rotationSpeed = value; } }
    public bool RotateOnUpdate { get { return _rotateOnUpdate; } set { _rotateOnUpdate = value; } }

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
        if (_rotateOnUpdate == false) { return; }

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

    public virtual void RotateWASD(int movementDir = 1)
    {
        float horizontal = movementDir == 1 ? CurrentPlayer.MoveDirection.x : CurrentPlayer.MoveDirection2.x;
        float vertical = movementDir == 1 ? CurrentPlayer.MoveDirection.y : CurrentPlayer.MoveDirection2.y;

        _currentAngle = _rotationalHumble.CalculateCurrentAngle(horizontal, vertical, _currentAngle, _rotationSpeed);

        SetRotationWASD();
    }

    private void SetRotationWASD()
    {
        _rotationalHumble.CalculateRotationWASD(_pivotTransform.position, RotatorTransform.position, _radius, _currentAngle, out Vector3 finalPos, out Quaternion finalRotation);

        RotatorTransform.position = finalPos;
        RotatorTransform.rotation = finalRotation;
    }

    public virtual void Rotate(int movementDir = 1)
    {
        Vector3 moveDir = movementDir == 1 ? CurrentPlayer.MoveDirection : CurrentPlayer.MoveDirection2;

        if (moveDir.magnitude == 0) { return; }

        _currentAngle = _rotationSpeed * moveDir.x * Time.deltaTime;
        RotatorTransform.RotateAround(_pivotTransform.position, Vector3.forward, -_currentAngle);
    }

    public void SetRotatorTransform(Transform newTransform)
    {
        RotatorTransform = newTransform;
    }
}
