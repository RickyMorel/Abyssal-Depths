using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Booster : RotationalInteractable
{
    #region Editor Fields

    [Header("Booster Stats")]
    [SerializeField] private float _acceleration = 20f;
    [SerializeField] private float _hoverAcceleration = 13f;
    [SerializeField] private float _boostImpulseForce = 50f;
    [SerializeField] private float _shipDrag = 0.1f;
    [SerializeField] private List<Gear> _gears = new List<Gear>();
    [SerializeField] private float _speedReductionTolerance = 8f;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private BoosterStabilizer _boosterStabilizer;
    [SerializeField] private BoosterStabilizer[] _boosterStabilizers;

    #endregion

    #region Private Variables

    private bool _isBoosting = false;
    private bool _isHovering = false;
    private bool _lockHovering = false;
    private int _currentGear = 0;
    private bool _recentlyChangedGear = false;
    private bool _isStuttering = false;
    private bool _canStutter = true;

    #endregion

    #region Public Properties

    public static event Action<bool> OnBoostUpdated;
    public static event Action<int> OnGearChanged;

    public bool RecentlyChangedGear => _recentlyChangedGear;

    public float Acceleration => _acceleration;

    public Rigidbody RB => _rb;

    public bool IsBoosting => _isBoosting;

    #endregion

    #region Getters and Setters

    public bool LockHovering { get { return _lockHovering; } set { _lockHovering = value; } }

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

        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();

        SetIsBoosting(false);
        StabilizeShip();
    }

    private void OnDestroy()
    {
        OnGearChanged -= HandleGearChanged;
    }

    public override void Update()
    {
        base.Update();

        if(CanUse == false) { return; }
       
        if (CurrentPlayer == null) 
        {
            SetIsBoosting(false);
            if (!_lockHovering) { SetIsHovering(false); }
            return;
        }

        SetIsBoosting(CurrentPlayer.IsUsing);
        SetIsHovering(CurrentPlayer.IsUsing_2 || _lockHovering);
        if (CurrentPlayer.PlayerInput.DetectDoubleTap()) { _lockHovering = !_lockHovering; }
    }

    private void FixedUpdate()
    {
        BoostShip();
        CheckGears();
        StabilizeShip();
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

        yield return new WaitForSeconds(Ship.Instance.BoostTimeAfterGearChange);

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

        if (_isBoosting)
        {
            _boosterStabilizer.Particles.Play();
            _boosterStabilizer.LightComponent.enabled = true;
        }
        else
        {
            _boosterStabilizer.Particles.Stop();
            _boosterStabilizer.LightComponent.enabled = false;
        }

        OnBoostUpdated?.Invoke(isBoosting);

        if (isBoosting)
            BoostImpulse();
    }

    public void SetIsHovering(bool isHovering)
    {
        //If value is the same, don't update
        if (_isHovering == isHovering) { return; }

        _isHovering = isHovering;

        foreach (BoosterStabilizer stabilizer in _boosterStabilizers)
        {
            if (_isHovering)
            {
                stabilizer.Particles.Play();
                stabilizer.LightComponent.enabled = true;
            }
            else 
            {
                stabilizer.Particles.Stop();
                stabilizer.LightComponent.enabled = false;
            }
        }
    }

    private void StabilizeShip()
    {
        if (!_isHovering) { return; }

        if (_rb.velocity == Vector3.zero) { return; }

        _rb.AddForce(-_rb.velocity.normalized * _rb.mass * _hoverAcceleration);
    }

    private void BoostImpulse()
    {
        _rb.AddForce((RotatorTransform.transform.up * _boostImpulseForce * _rb.mass), ForceMode.Impulse);
    }

    private void BoostShip()
    {
        if (!_isBoosting || _recentlyChangedGear) { return; }

        if (IsBroken()) 
        { 
            if (_canStutter) { StartCoroutine(SetStutter()); }
        }

        if (_isStuttering) { return; }

        _rb.AddForce(RotatorTransform.transform.up * _acceleration * _rb.mass);

        _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, Ship.Instance.TopSpeed);
    }

    private IEnumerator SetStutter()
    {
        _canStutter = false;
        _isStuttering = true;

        yield return new WaitForSeconds(Random.Range(0.2f, 0.8f));

        _isStuttering = false;

        yield return new WaitForSeconds(Random.Range(0.3f, 2f));

        _canStutter = true;
    }

    public void TurnOffEngine()
    {
        _lockHovering = false;

        if (CurrentPlayer != null) { CurrentPlayer.CheckExitInteraction(); }
    }

}

#region Helper Classes

[System.Serializable]
public class Gear
{
    public float MaxSpeed;
}

[System.Serializable]
public class BoosterStabilizer
{
    public ParticleSystem Particles;
    public Light LightComponent;
}

#endregion