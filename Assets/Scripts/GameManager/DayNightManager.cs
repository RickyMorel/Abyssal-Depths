using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightManager : MonoBehaviour
{
    #region Editor Fields

    [Tooltip("This is measured in seconds")]
    [SerializeField] private int _howLongTheDayLast = 6;
    [Tooltip("This is measured in seconds")]
    [SerializeField] private int _howLongTheNightLast = 6;
    [SerializeField] private float _lowFogDensity = 0.01f;
    [SerializeField] private float _highFogDensity = 0.03f;
    [Tooltip("Lower this value to increase transition time")]
    [SerializeField] private float _fogTransitionSpeed = 1;

    #endregion

    #region Private Variables

    private static DayNightManager _instance;
    private bool _isNightTime = false;
    private int _dayCount = 2;
    private float _dayTimer = 0;
    private float _nightTimer = 0;
    private GameObject[] _fogDependantObjects;
    private List<Light> _lights = new List<Light>();
    private List<float> _lightsOriginalIntensity = new List<float>();
    private List<ParticleSystem> _gameObjectsToDeactivateAtNight = new List<ParticleSystem>();

    #endregion

    #region Public Properties

    public static DayNightManager Instance { get { return _instance; } }
    public bool IsNightTime => _isNightTime;
    public int DayCount => _dayCount;
    public event Action OnCycleChange;

    #endregion

    #region Unity Loops

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        _fogDependantObjects = GameObject.FindGameObjectsWithTag("FogDependant");

        for (int i = 0; i < _fogDependantObjects.Length; i++)
        {
            if (_fogDependantObjects[i].GetComponent<Light>()) { _lights.Add(_fogDependantObjects[i].GetComponent<Light>()); }
            if (_fogDependantObjects[i].GetComponent<ParticleSystem>()) { _gameObjectsToDeactivateAtNight.Add(_fogDependantObjects[i].GetComponent<ParticleSystem>()); }
        }

        foreach (Light light in _lights)
        {
            _lightsOriginalIntensity.Add(light.intensity);
        }
    }

    private void Update()
    {
        DayNightCycle();
    }

    #endregion

    private void DayNightCycle()
    {
        if (!_isNightTime)
        {
            DayEffectsTransition();
        }

        if (_isNightTime)
        {
            NightEffectsTransition();
        }
    }

    private void DayEffectsTransition()
    {
        _dayTimer += Time.deltaTime;

        RenderSettings.fogDensity = Mathf.Lerp(_highFogDensity, _lowFogDensity, _dayTimer * _fogTransitionSpeed);

        EnableFogEffect();

        if (_dayTimer >= _howLongTheDayLast)
        {
            _isNightTime = true;
            OnCycleChange?.Invoke();
            _dayTimer = 0;
        }
    }

    private void NightEffectsTransition()
    {
        _nightTimer += Time.deltaTime;

        RenderSettings.fogDensity = Mathf.Lerp(_lowFogDensity, _highFogDensity, _nightTimer * _fogTransitionSpeed);

        EnableFogEffect();

        if (_nightTimer >= _howLongTheNightLast)
        {
            _isNightTime = false;
            OnCycleChange?.Invoke();
            _nightTimer = 0;
            _dayCount += 1;
        }
    }

    private void EnableFogEffect()
    {
        if (!_isNightTime)
        {
            for (int i = 0; i < _lights.Count; i++)
            {
                _lights[i].intensity = Mathf.Lerp(0, _lightsOriginalIntensity[i], _dayTimer * _fogTransitionSpeed);
            }
            foreach (ParticleSystem godRay in _gameObjectsToDeactivateAtNight)
            {
                godRay.gameObject.SetActive(true);
            }
        }
        if (_isNightTime)
        {
            for (int i = 0; i < _lights.Count; i++)
            {
                _lights[i].intensity = Mathf.Lerp(_lightsOriginalIntensity[i], 0, _nightTimer * _fogTransitionSpeed);
            }
            foreach (ParticleSystem godRay in _gameObjectsToDeactivateAtNight)
            {
                godRay.gameObject.SetActive(false);
            }
        }
    }
}