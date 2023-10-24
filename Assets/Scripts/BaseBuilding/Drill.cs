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
        if (!CanDrill())
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
            float drillingTime = DayNightManager.Instance.HowLongTheDayLasts - DayNightManager.Instance.NightWarningTime;
            _drillGameObject.transform.position = Vector3.Lerp(_drillPositionBeforeMoving, _drillStopsTransforms[_currentPositionToGoToIndex].position, _timer / drillingTime);
        }
        else if (!CanDrill() && DayNightManager.Instance.CurrentTime == DayNightManager.DayNightTime.AboutToBeNight)
        {
            if (_drillStopsTransforms.Length - 1 == _currentPositionToGoToIndex) { return; }
            
            _drillGameObject.transform.position = _drillStopsTransforms[_currentPositionToGoToIndex].position;
            _currentPositionToGoToIndex += 1;
            _timer = 0;
            _drillPositionBeforeMoving = _drillGameObject.transform.position;
            OnDestroyCurrentRock?.Invoke();
        }
    }

    private void DrillingSfx()
    {
        GameAudioManager.Instance.PlaySound(GameAudioManager.Instance.DrillingSfx, transform.position);
    }

    public static bool CanDrill()
    {
        if (DayNightManager.Instance.DayCount < 2) { return false; }

        if (DayNightManager.Instance.CurrentTime == DayNightManager.DayNightTime.AboutToBeNight || DayNightManager.Instance.CurrentTime == DayNightManager.DayNightTime.NightTime) { return false; }
        
        return true;
    }
}