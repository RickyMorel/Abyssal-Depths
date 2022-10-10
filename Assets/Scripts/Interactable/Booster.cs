using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : RotationalInteractable
{

    #region Editor Fields

    [Header("Booster Stats")]
    [SerializeField] private float _acceleration = 1.0f;
    [SerializeField] private float _boostImpulseForce = 50f;
    [SerializeField] private float _topSpeed = 200f;
    [SerializeField] private float _shipDrag = 0.1f;
    [SerializeField] private List<Gear> _gears = new List<Gear>();
    [SerializeField] private float _speedReductionTolerance = 8f;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private ParticleSystem _boosterParticle;

    #endregion

    #region Private Variables

    private bool _isBoosting = false;
    private int _currentGear = 0;
    private bool _recentlyChangedGear = false;

    #endregion

    #region Public Properties

    public static event Action<bool> OnBoostUpdated;
    public static event Action<int> OnGearChanged;

    public bool RecentlyChangedGear => _recentlyChangedGear;

    #endregion

    #region Unity Loops

    public override void Awake()
    {
        base.Awake();

        OnGearChanged += HandleGearChanged;
    }

    public override void Start()
    {
        base.Start();

        _rb.drag = _shipDrag;
    }

    private void OnDestroy()
    {
        OnGearChanged -= HandleGearChanged;
    }

    public override void Update()
    {
        base.Update();
       
        if (_currentPlayer == null) { return; }
        
        if (!_currentPlayer.IsShooting) { SetIsBoosting(false); return; }

        SetIsBoosting(true);
    }

    private void FixedUpdate()
    {
        BoostShip();
        CheckGears();

        _boosterParticle.Stop();
    }

    #endregion

    #region Gears

    private void HandleGearChanged(int gear)
    {
        StartCoroutine(ChangedGearCoroutine());
    }

    private IEnumerator ChangedGearCoroutine()
    {
        _recentlyChangedGear = true;

        yield return new WaitForSeconds(0.5f);

        _recentlyChangedGear = false;
    }

    private void CheckGears()
    {
        if (_currentGear < (_gears.Count - 1) && _rb.velocity.magnitude > _gears[_currentGear].MaxSpeed)
        {
            _currentGear = Mathf.Clamp(_currentGear + 1, 0, _gears.Count - 1);
            OnGearChanged?.Invoke(_currentGear);
        }
        else if (_currentGear != 0 && _rb.velocity.magnitude + _speedReductionTolerance < _gears[_currentGear - 1].MaxSpeed)
        {
            _currentGear = Mathf.Clamp(_currentGear - 1, 0, _gears.Count - 1);
            OnGearChanged?.Invoke(_currentGear);
        }
    }

    #endregion

    private void SetIsBoosting(bool isBoosting)
    {
        //If value is the same, don't update
        if(_isBoosting == isBoosting) { return; }

        _isBoosting = isBoosting;

        OnBoostUpdated?.Invoke(isBoosting);

        if (isBoosting)
            BoostImpulse();
    }

    private void BoostImpulse()
    {
        _rb.AddForce(-(RotatorTransform.transform.up * _boostImpulseForce * _rb.mass), ForceMode.Impulse);
    }

    private void BoostShip()
    {
        if (!_isBoosting || _recentlyChangedGear) { return; }

        _boosterParticle.Play();

        _rb.AddForce(-(RotatorTransform.transform.up * _acceleration * _rb.mass));

        _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, _topSpeed);
    }

    #region Landing

    #endregion
}

#region Helper Classes

[System.Serializable]
public class Gear
{
    public float MaxSpeed;
}

#endregion