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
    private bool _activateTimer = false;
    private int _dayCount = 2;
    private float _universalTimer = 0;
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
        GetAllNecessaryObjectsAndComponents();

        Invoke(nameof(NightEffectsTransition), _howLongTheDayLast);
    }

    private void OnEnable()
    {
        OnCycleChange += DayNightCycle;
    }

    private void OnDisable()
    {
        OnCycleChange -= DayNightCycle;
    }

    private void Update()
    {
        if (_activateTimer) { Lerps(); }
    }

    #endregion

    private void GetAllNecessaryObjectsAndComponents()
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

    private void DayNightCycle()
    {
        if (_isNightTime)
        {
            Invoke(nameof(DayEffectsTransition), _howLongTheNightLast);
        }
        if (!_isNightTime)
        {
            Invoke(nameof(NightEffectsTransition), _howLongTheDayLast);
        }
    }

    private void DayEffectsTransition()
    {
        _isNightTime = false;
        OnCycleChange?.Invoke();
        EnableFogEffect();
    }

    private void NightEffectsTransition()
    {
        _isNightTime = true;
        OnCycleChange?.Invoke();
        _dayCount += 1;
        EnableFogEffect();
    }

    private void EnableFogEffect()
    {
        _activateTimer = true;

        if (!_isNightTime)
        {
            foreach (ParticleSystem godRay in _gameObjectsToDeactivateAtNight)
            {
                godRay.gameObject.SetActive(true);
            }
        }
        if (_isNightTime)
        {
            foreach (ParticleSystem godRay in _gameObjectsToDeactivateAtNight)
            {
                godRay.gameObject.SetActive(false);
            }
        }
    }

    private void Lerps()
    {
        _universalTimer += Time.deltaTime * _fogTransitionSpeed;

        if (_isNightTime)
        {
            RenderSettings.fogDensity = Mathf.Lerp(_lowFogDensity, _highFogDensity, _universalTimer);
            for (int i = 0; i < _lights.Count; i++)
            {
                _lights[i].intensity = Mathf.Lerp(_lightsOriginalIntensity[i], 0, _universalTimer);
            }
            
        }
        else
        {
            RenderSettings.fogDensity = Mathf.Lerp(_highFogDensity, _lowFogDensity, _universalTimer);
            for (int i = 0; i < _lights.Count; i++)
            {
                _lights[i].intensity = Mathf.Lerp(0, _lightsOriginalIntensity[i], _universalTimer);
            }
        }

        if (_universalTimer > 1) { _activateTimer = false; _universalTimer = 0; }
    }
}