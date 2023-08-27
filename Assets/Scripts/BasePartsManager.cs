using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Analytics;

public class BasePartsManager : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private BasePartBaseLocation _basePartLocation;

    #endregion

    #region Private Variables

    private static BasePartsManager _instance;
    private int _currentLocationIndex;

    #endregion

    #region Public Properties

    public static BasePartsManager Instance { get { return _instance; } }

    #endregion

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

        _basePartLocation.LoadQueues();
    }

    public Transform GetLocation(BasePartType partType)
    {
        return _basePartLocation.GetLocation(partType);
    }

    [System.Serializable]
    public class BasePartBaseLocation
    {
        public List<Transform> TurretLocations = new List<Transform>();
        public List<Transform> ForgeLocations = new List<Transform>();
        public List<Transform> GateLocations = new List<Transform>();

        private Queue<Transform> TurretLocationsQueue = new Queue<Transform>();
        private Queue<Transform> ForgeLocationsQueue = new Queue<Transform>();
        private Queue<Transform> GateLocationsQueue = new Queue<Transform>();

        //public BasePartBaseLocation()
        //{
        //    GameObject[] turretLocations = GameObject.FindGameObjectsWithTag("TurretLocation");

        //    foreach (GameObject location in turretLocations)
        //    {
        //        TurretLocationsQueue.Enqueue(location.transform);
        //    }
        //}

        public void LoadQueues()
        {
            foreach (Transform location in TurretLocations)
            {
                TurretLocationsQueue.Enqueue(location);
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
}

public enum BasePartType
{
    None,
    Turret,
    Forge,
    Gate
}
