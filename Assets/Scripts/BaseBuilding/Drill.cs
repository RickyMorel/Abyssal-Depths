using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drill : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject _drillHead;
    [SerializeField] private float _drillRotationSpeed;
    [SerializeField] private Vector3 _rotation;
    [SerializeField] private float _timeToReachRotationMaxSpeedMultiplier = 1;

    #endregion

    #region Private Variables

    private float _rotationSpeed;

    #endregion

    #region Unity Loops

    private void Update()
    {
        if (!DayNightManager.Instance.IsNightTime) { return; }
        else
        {
            if (_rotationSpeed != 0) { _rotationSpeed = 0; }
        }

        DrillRotation();

        if (_rotationSpeed >= 1) { return; }

        _rotationSpeed += Time.deltaTime * _timeToReachRotationMaxSpeedMultiplier;
    }

    #endregion

    private void DrillRotation()
    {
        _drillHead.transform.Rotate(_rotation * Time.deltaTime * _drillRotationSpeed * _rotationSpeed);
    }
}