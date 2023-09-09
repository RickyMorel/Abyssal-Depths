using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drill : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject _drillGameObject;
    [SerializeField] private GameObject _drillHead;
    [SerializeField] private float _drillRotationSpeed;
    [SerializeField] private Vector3 _rotation;
    [SerializeField] private float _timeToReachRotationMaxSpeedMultiplier = 1;
    [SerializeField] private float _timeToReachMovementMaxSpeedMultiplier = 1;
    [SerializeField] private Transform[] _drillStopsTransforms;

    #endregion

    #region Private Variables

    private float _rotationSpeed;
    private float _timer = 0;
    private Vector3 _drillPositionBeforeMoving;
    private int _currentPositionToGoToIndex = 0;

    #endregion

    #region Public Properties

    public event Action OnDestroyCurrentRock;

    #endregion

    #region Unity Loops

    private void Start()
    {
        _drillPositionBeforeMoving = _drillGameObject.transform.position;
    }

    private void Update()
    {
        DrillingNightActivity();

        if (CanDrill()) { _timer += Time.deltaTime; }
    }

    private void OnEnable()
    {
        DayNightManager.Instance.OnCycleChange += DrillingSfx;
    }

    private void OnDisable()
    {
        DayNightManager.Instance.OnCycleChange -= DrillingSfx;
    }

    #endregion

    private void DrillingNightActivity()
    {
        Rotation();
        Movement();
    }

    private void Rotation()
    {
        if (!CanDrill() && _timer - DayNightManager.Instance.NightWarningTime >= 0)
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
        if (CanDrill())
        {
            if (_timer - DayNightManager.Instance.NightWarningTime >= 0) { return; }

            float drillingTime = DayNightManager.Instance.HowLongTheDayLasts - DayNightManager.Instance.NightWarningTime;
            _drillGameObject.transform.position = Vector3.Lerp(_drillPositionBeforeMoving, _drillStopsTransforms[_currentPositionToGoToIndex].position, (_timer / drillingTime));

            if (_drillGameObject.transform.position == _drillStopsTransforms[_currentPositionToGoToIndex].position && _drillStopsTransforms.Length -1 != _currentPositionToGoToIndex) 
            { 
                _currentPositionToGoToIndex += 1; 
                _timer = 0;
                _drillPositionBeforeMoving = _drillGameObject.transform.position;
                OnDestroyCurrentRock?.Invoke();
            }
        }
    }

    private void DrillingSfx()
    {
        GameAudioManager.Instance.PlaySound(GameAudioManager.Instance.DrillingSfx, transform.position);
    }

    public static bool CanDrill()
    {
        if (DayNightManager.Instance.DayCount < 2) { return false; }

        if (DayNightManager.Instance.IsNightTime) { return false; }

        return true;
    }
}