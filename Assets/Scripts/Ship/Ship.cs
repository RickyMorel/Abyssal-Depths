using System;
using UnityEngine;

[RequireComponent(typeof(ShipHealth))]
[RequireComponent(typeof(ShipFastTravel))]
[RequireComponent(typeof(ShipData))]
[RequireComponent(typeof(Rigidbody))]
public class Ship : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private ShipStatsSO _shipStatsSO;

    #endregion

    #region Private Variables

    private static Ship _instance;
    private ShipHealth _health;
    private ShipData _shipData;
    private ShipFastTravel _shipFastTravel;
    private Rigidbody _rb;

    private float _topSpeed;
    private float _boostTimeAfterGearChange;
    private float _timeTillDeath;

    #endregion

    #region Public Properties

    public static Ship Instance { get { return _instance; } }
    public ShipStatsSO ShipStatsSO => _shipStatsSO;
    public ShipHealth ShipHealth => _health;
    public ShipData ShipData => _shipData;
    public ShipFastTravel ShipFastTravel => _shipFastTravel;
    public Rigidbody Rb => _rb;
    public float TopSpeed => _topSpeed;
    public float BoostTimeAfterGearChange => _boostTimeAfterGearChange;
    public float TimeTillDeath => _timeTillDeath;

    public event Action OnRespawn;

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
    }

    private void Start()
    {
        _health = GetComponent<ShipHealth>();
        _shipData = GetComponent<ShipData>();
        _rb = GetComponent<Rigidbody>();
        _shipFastTravel = GetComponent<ShipFastTravel>();

        SetShipStats();
    }

    private void SetShipStats()
    {
        _topSpeed = _shipStatsSO.TopSpeed;
        _boostTimeAfterGearChange = _shipStatsSO.BoostTimeAfterGearChange;
        _timeTillDeath = _shipStatsSO.TimeTillDeath;
        _health.SetMaxHealth(_shipStatsSO.MaxHealth);
        _health.MinCrashSpeed = _shipStatsSO.MinCrashSpeed;
        _health.CrashDamageMultiplier = _shipStatsSO.CrashDamageMultiplier;
    }

    public void AddForceToShip(Vector3 force, ForceMode forceMode)
    {
        if(forceMode != ForceMode.VelocityChange)
        {
            _rb.AddForce(force * _rb.mass, forceMode);
        }
        else
        {
            _rb.AddForce(force, forceMode);
        }
    }

    public void FreezeShip(bool isFreezed)
    {
        _rb.isKinematic = isFreezed;
    }

    public void FireRespawnEvent()
    {
        OnRespawn?.Invoke();
    }
}
