using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RenderSettings = UnityEngine.RenderSettings;

public class DayNightManager : MonoBehaviour
{
    #region Editor Fields

    [Header("Lights")]
    [SerializeField] private List<Light> _directionalLights = new List<Light>();

    [Header("Transition Variables")]
    [Tooltip("This is measured in seconds")]
    [SerializeField] private int _howLongTheDayLast = 6;
    [SerializeField] private int _nightWarningTime = 20;
    [Tooltip("This is measured in seconds")]
    [SerializeField] private int _howLongTheNightLast = 6;
    [SerializeField] private int _night = 6;
    [SerializeField] private float _lowFogDensity = 0.01f;
    [SerializeField] private float _highFogDensity = 0.03f;
    [Tooltip("Lower this value to increase transition time")]
    [SerializeField] private float _fogTransitionSpeed = 1;
    [SerializeField] private DayNightSO _dayNightSO;

    #endregion

    #region Private Variables

    private static DayNightManager _instance;
    private bool _isNightTime = false;
    private bool _activateTimer = false;
    private int _dayCount = 1;
    private float _universalTimer = 0;
    private GameObject[] _fogDependantObjects;
    private List<Light> _lights = new List<Light>();
    private List<float> _lightsOriginalIntensity = new List<float>();
    private List<ParticleSystem> _gameObjectsToDeactivateAtNight = new List<ParticleSystem>();
    private DayNightTime _currentTime;

    #endregion

    #region Public Properties

    public static DayNightManager Instance { get { return _instance; } }
    public int HowLongTheDayLasts => _howLongTheDayLast;
    public int HowLongTheNightLast => _howLongTheNightLast;
    public int NightWarningTime => _nightWarningTime;
    public bool IsNightTime => _isNightTime;
    public int DayCount => _dayCount;
    public DayNightTime CurrentTime => _currentTime;
    public event Action OnCycleChange;
    public event Action OnNightComingWarning;

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

        GameStatsPanelUI.Instance.UpdateDays(_dayCount);
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

        if (Input.GetKeyDown(KeyCode.L)) { Time.timeScale = 8f; }
        if (Input.GetKeyDown(KeyCode.J)) { Time.timeScale = 1f; }
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

        Light[] allLights = FindObjectsOfType<Light>();

        foreach (Light light in allLights)
        {
            if(light.type != LightType.Directional) { continue; }

            _directionalLights.Add(light);  
        }

        _directionalLights = _directionalLights.OrderBy(light => light.gameObject.name).ToList();
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
            Invoke(nameof(NightComingWarning), _howLongTheDayLast - _nightWarningTime);
        }
    }

    private void NightComingWarning()
    {
        _currentTime = DayNightTime.AboutToBeNight;
        OnNightComingWarning?.Invoke();
    }

    private void DayEffectsTransition()
    {
        _isNightTime = false;
        _currentTime = DayNightTime.DayTime;
        OnCycleChange?.Invoke();
        EnableFogEffect();
    }

    private void NightEffectsTransition()
    {
        _isNightTime = true;
        _currentTime = DayNightTime.NightTime;
        OnCycleChange?.Invoke();
        _dayCount += 1;
        GameStatsPanelUI.Instance.UpdateDays(_dayCount);
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

    private void SetLightColors()
    {
        List<Color> color_1 = _isNightTime ? new List<Color>(_dayNightSO.DayLightColors) : new List<Color>(_dayNightSO.NightLightColors);
        List<Color> color_2 = _isNightTime ? new List<Color>(_dayNightSO.NightLightColors) : new List<Color>(_dayNightSO.DayLightColors);

        _directionalLights[0].color = Color.Lerp(color_1[0], color_2[0], _universalTimer);
        _directionalLights[1].color = Color.Lerp(color_1[1], color_2[1], _universalTimer);
        _directionalLights[2].color = Color.Lerp(color_1[2], color_2[2], _universalTimer);
        RenderSettings.fogColor = Color.Lerp(color_1[3], color_2[3], _universalTimer);
    }

    private void Lerps()
    {
        _universalTimer += Time.deltaTime * _fogTransitionSpeed;

        SetLightColors();

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

    public enum DayNightTime { DayTime, AboutToBeNight, NightTime }
}