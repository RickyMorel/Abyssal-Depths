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
        if (CurrentPlayer == null && CurrentAngle != 0) { FinishRotateWhenPlayerIsNull(); }

        if (CurrentPlayer == null) { SetIsBoosting(false); return; }

        if (!CurrentPlayer.IsUsing) { SetIsBoosting(false); return; }

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
        if(CurrentAngle == 0f) { return; }

        if(CurrentPlayer != null && (_isBoosting || CurrentPlayer.MoveDirection.magnitude > 0) && CurrentAngle < _topSpeed) { return; }

        CurrentAngle -= CurrentAngle * _pickaxeDrag * Time.deltaTime;
    }

    private void BoostPickaxe()
    {
        _topSpeed = _isBoosting ? _boostTopSpeed : _normalTopSpeed;
    }

    public void ApplyImpactForce()
    {
        if(CurrentPlayer == null) { return; }

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
        float acceleration = RotationSpeed * CurrentPlayer.MoveDirection.x * Time.deltaTime;

        CurrentAngle = CurrentAngle + acceleration;

        //Slow down if surpassed top speed
        CurrentAngle = Mathf.Clamp(CurrentAngle, -_topSpeed, _topSpeed);

        RotatorTransform.RotateAround(PivotTransform.position, Vector3.forward, -CurrentAngle);
    }

    public virtual void FinishRotateWhenPlayerIsNull()
    {
        RotatorTransform.RotateAround(PivotTransform.position, Vector3.forward, -CurrentAngle);
    }
}
