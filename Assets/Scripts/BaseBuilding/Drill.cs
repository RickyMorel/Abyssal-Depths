using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drill : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject _drillGameObject;
    [SerializeField] private GameObject _drillHead;
    [SerializeField] private float _drillRotationSpeed;
    [SerializeField] private float _movementMaxSpeed;
    [SerializeField] private Vector3 _rotation;
    [SerializeField] private float _timeToReachRotationMaxSpeedMultiplier = 1;
    [SerializeField] private float _timeToReachMovementMaxSpeedMultiplier = 1;
    [SerializeField] private Transform[] _drillStopsTransforms;

    #endregion

    #region Private Variables

    private float _rotationSpeed;
    private float _movementSpeed;

    #endregion

    #region Unity Loops

    private void Update()
    {
        DrillingNightActivity();
    }

    #endregion

    private void DrillingNightActivity()
    {
        Rotation();
        Movement();
    }

    private void Rotation()
    {
        if (!DayNightManager.Instance.IsNightTime)
        {
            if (_rotationSpeed <= 0) { return; }

            _rotationSpeed -= Time.deltaTime * _timeToReachRotationMaxSpeedMultiplier;

            _drillHead.transform.Rotate(_rotation * Time.deltaTime * _drillRotationSpeed * _rotationSpeed);
        }
        else
        {
            _drillHead.transform.Rotate(_rotation * Time.deltaTime * _drillRotationSpeed * _rotationSpeed);

            if (_rotationSpeed >= 1) { return; }

            _rotationSpeed += Time.deltaTime * _timeToReachRotationMaxSpeedMultiplier;
        }
    }

    private void Movement()
    {
        if (!DayNightManager.Instance.IsNightTime)
        {
            if (_movementSpeed <= 0) { return; }

            _movementSpeed -= Time.deltaTime * _timeToReachMovementMaxSpeedMultiplier;

            _drillGameObject.transform.position = Vector3.MoveTowards(_drillGameObject.transform.position, _drillStopsTransforms[0].position, _movementSpeed * _movementMaxSpeed);
        }
        else
        {
            _drillGameObject.transform.position = Vector3.MoveTowards(_drillGameObject.transform.position, _drillStopsTransforms[0].position, _movementSpeed * _movementMaxSpeed);

            if (_movementSpeed >= 1) { return; }

            _movementSpeed += Time.deltaTime * _timeToReachMovementMaxSpeedMultiplier;
        }
    }
}