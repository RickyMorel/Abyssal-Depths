using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickaxe : RotationalInteractable
{
    #region Editor Fields

    [SerializeField] private float _pickaxeDrag = 0.1f;
    [SerializeField] private float _normalTopSpeed = 200f;
    [SerializeField] private float _boostTopSpeed = 200f;
    [SerializeField] private float _damageMultiplier = 2f;
    [SerializeField] private ParticleSystem _pickaxeBoostParticles;

    #endregion

    #region Private Variables

    private float _topSpeed;
    private bool _isBoosting = false;

    #endregion

    #region Unity Loops

    public override void Update()
    {
        base.Update();

        //Finish moving even when player gets off
        if (_currentPlayer == null && CurrentAngle != 0) { FinishRotateWhenPlayerIsNull(); }

        if (_currentPlayer == null) { SetIsBoosting(false); return; }

        if (!_currentPlayer.IsUsing) { SetIsBoosting(false); return; }

        SetIsBoosting(true);
    }

    private void FixedUpdate()
    {
        BoostPickaxe();
        ApplyDrag();
        if(CurrentAngle < 0) { SetIsBoosting(false); }
    }   

    #endregion

    private void SetIsBoosting(bool isBoosting)
    {
        //If value is the same, don't update
        if (_isBoosting == isBoosting) { return; }

        //Can't Boost Backwards
        _isBoosting = CurrentAngle < 0 ? false : isBoosting;

        if (_isBoosting)
            _pickaxeBoostParticles.Play();  
        else
            _pickaxeBoostParticles.Stop();
    }

    private void ApplyDrag()
    {
        if(CurrentAngle == 0) { return; }

        if(_currentPlayer != null && (_isBoosting || _currentPlayer.MoveDirection.magnitude > 0) && CurrentAngle < _topSpeed) { return; }

        CurrentAngle -= CurrentAngle * _pickaxeDrag * Time.deltaTime;
    }

    private void BoostPickaxe()
    {
        _topSpeed = _isBoosting ? _boostTopSpeed : _normalTopSpeed;
    }

    public void ApplyImpactForce()
    {
        bool applyForwardForce = CurrentAngle >= 0;
        float force = applyForwardForce ? -RotationSpeed : RotationSpeed;

        CurrentAngle = force;
    }

    public float GetHitSpeed()
    {
        if (!_isBoosting) return 0;

        return CurrentAngle * _damageMultiplier;
    }

    public override void Rotate()
    {
        float acceleration = RotationSpeed * _currentPlayer.MoveDirection.x * Time.deltaTime;

        //Start slowing down if surpassed top speed
        if (Mathf.Abs(CurrentAngle) > _topSpeed) { acceleration = 0; }

        CurrentAngle = CurrentAngle + acceleration;

        RotatorTransform.RotateAround(PivotTransform.position, Vector3.forward, -CurrentAngle);
    }

    public virtual void FinishRotateWhenPlayerIsNull()
    {
        RotatorTransform.RotateAround(PivotTransform.position, Vector3.forward, -CurrentAngle);
    }
}
