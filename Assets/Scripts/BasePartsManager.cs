using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Analytics;
using System;

public class BasePartsManager : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private List<BasePartBaseLocation> _basePartLocations = new List<BasePartBaseLocation>();

    #endregion

    #region Private Variables

    private static BasePartsManager _instance;
    private int _currentLocationIndex = -1;

    #endregion

    #region Public Properties

    public static BasePartsManager Instance { get { return _instance; } }

    public event Action OnLocationChanged;

    #endregion

    #region Unity Loops

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        LoadLocationQueues();
    }

    private void Start()
    {
        DayNightManager.Instance.OnNightComingWarning += HandleCycleChange;
    }

    private void OnDestroy()
    {
        DayNightManager.Instance.OnNightComingWarning -= HandleCycleChange;
    }

    #endregion

    public bool HasNextLocation()
    {
        Debug.Log($"HasNextLocation: {_currentLocationIndex} < {_basePartLocations.Count-1}" + (_currentLocationIndex < _basePartLocations.Count - 1));
        return _currentLocationIndex <= _basePartLocations.Count - 1;
    }

    private void HandleCycleChange()
    {
        if(!HasNextLocation()) { return; }

        _currentLocationIndex++;

        OnLocationChanged?.Invoke();
    }

    private void LoadLocationQueues()
    {
        foreach (BasePartBaseLocation _basePartLocation in _basePartLocations)
        {
            _basePartLocation.LoadQueues();
        }
    }

    public Transform GetLocation(BasePartType partType)
    {
        if (BuildingUpgradeUI.Instance != null) { BuildingUpgradeUI.Instance.EnableUpgradesPanel(false); }
        if (BuildingFarmUI.Instance != null) { BuildingFarmUI.Instance.EnablePanel(false); }

        return _basePartLocations[_currentLocationIndex].GetLocation(partType);
    }

    #region Helper Classes

    [System.Serializable]
    public class BasePartBaseLocation
    {
        public List<Transform> TurretLocations = new List<Transform>();
        public List<Transform> ForgeLocations = new List<Transform>();
        public List<Transform> GateLocations = new List<Transform>();

        private Queue<Transform> TurretLocationsQueue = new Queue<Transform>();
        private Queue<Transform> ForgeLocationsQueue = new Queue<Transform>();
        private Queue<Transform> GateLocationsQueue = new Queue<Transform>();

        public void LoadQueues()
        {
            foreach (Transform location in TurretLocations)
            {
                TurretLocationsQueue.Enqueue(location);
            }
            foreach (Transform location in ForgeLocations)
            {
                ForgeLocationsQueue.Enqueue(location);
            }
            foreach (Transform location in GateLocations)
            {
                GateLocationsQueue.Enqueue(location);
            }
        }

        public Transform GetLocation(BasePartType partType)
        {
            switch (partType)
            {
                case BasePartType.Turret:
                    return TurretLocationsQueue.Dequeue();
                case BasePartType.Forge:
                    return ForgeLocationsQueue.Dequeue();
                case BasePartType.Gate:
                    return GateLocationsQueue.Dequeue();
            }

            return null;
        }
    }

    #endregion
}

public enum BasePartType
{
    None,
    Turret,
    Forge,
    Gate
}
