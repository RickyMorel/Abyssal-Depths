using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponWithRope : MeleeWeapon
{
    #region Editor Fields

    [SerializeField] protected Transform _moveToTransform;
    [SerializeField] protected Transform _handleTransform;
    [SerializeField] protected Rigidbody _weaponHeadRb;
    [SerializeField] protected Transform _bendPoint;
    [SerializeField] protected float _flyingSpeed;

    #endregion

    #region Private Variables

    private Transform _middlePointTransform;
    [SerializeField] protected ThrowState _throwState = ThrowState.Attached;
    protected float _timePassedReturning;
    protected Vector3 _originalWeaponHeadPosition;
    protected Quaternion _originalWeaponHeadRotation;

    #endregion

    #region Unity Loops

    public override void Start()
    {
        base.Start();

        _originalWeaponHeadPosition = _weaponHeadRb.transform.localPosition;
        _originalWeaponHeadRotation = _weaponHeadRb.transform.localRotation;

        _middlePointTransform = new GameObject("MiddlePointTransform").transform;
    }

    public override void Update()
    {
        base.Update();

        if (_weaponHeadRb.transform.parent != null)
        {
            _weaponHeadRb.transform.localRotation = Quaternion.Euler(0, 270, 270);
        }
        else
        {
            _weaponHeadRb.transform.rotation = Quaternion.Euler(0, CalculateAngleY(), CalculateAngle() - 90);
        }
    }

    public virtual void LateUpdate()
    {
        DrawRope();
    }

    #endregion

    public override void Shoot()
    {
        if (_throwState != ThrowState.Attached)
        {
            if (_throwState == ThrowState.Arrived || _throwState == ThrowState.Stuck || _throwState == ThrowState.GrabbingEnemy) { ReturnWeaponHead(); }

            return;
        }

        _throwState = ThrowState.Throwing;
        _weaponHeadRb.isKinematic = false;
        _weapon.ShouldRotate = false;

        _weaponHeadRb.transform.SetParent(null);
    }

    public virtual void DrawRope()
    {
        _middlePointTransform.position = new Vector3((_weaponHeadRb.transform.position.x + _handleTransform.position.x) / 2, (_weaponHeadRb.transform.position.y + _handleTransform.position.y) / 2, (_weaponHeadRb.transform.position.z + _handleTransform.position.z) / 2);
        _middlePointTransform.LookAt(_weaponHeadRb.transform);

        _bendPoint.LookAt(_weaponHeadRb.transform);

        float height, distanceA, distanceB, distanceC;

        distanceA = Mathf.Clamp(Vector3.Distance(_bendPoint.position, _handleTransform.position), 1f, float.MaxValue);
        distanceB = Vector3.Distance(_bendPoint.position, _weaponHeadRb.position);
        distanceC = Mathf.Sqrt(distanceA * distanceA + distanceB * distanceB);
        height = (distanceA * distanceB) / distanceC;

        if (Vector3.Distance(_bendPoint.position, _handleTransform.position) > 10 || Vector3.Distance(_bendPoint.position, _weaponHeadRb.transform.position) > 10)
        {
            _bendPoint.position = Vector3.MoveTowards(_bendPoint.position, _middlePointTransform.position, Time.deltaTime * 16);

            return;
        }

        Vector3 bendPointTargetPosition = Vector3.zero;

        if (_weaponHeadRb.velocity.y < 0)
        {
            bendPointTargetPosition = _middlePointTransform.position + _middlePointTransform.up * height;
        }
        else if (_weaponHeadRb.velocity.y > 0)
        {
            bendPointTargetPosition = _middlePointTransform.position + _middlePointTransform.up * -height;
        }

        _bendPoint.position = Vector3.MoveTowards(_bendPoint.position, bendPointTargetPosition, Time.deltaTime * 8);
    }

    public void ThrowWeaponHead()
    {
        if (_throwState == ThrowState.Arrived) { return; }

        Transform moveToCurrentPosition = _moveToTransform;

        if (_throwState == ThrowState.Throwing)
        {
            Vector3 forceDir = _moveToTransform.position - _weaponHeadRb.transform.position;
            _weaponHeadRb.AddForce(forceDir.normalized * _flyingSpeed, ForceMode.Force);
        }

        if (_throwState == ThrowState.Throwing && Vector3.Distance(_weaponHeadRb.transform.position, moveToCurrentPosition.position) < 2f)
        {
            _throwState = ThrowState.Arrived;
        }

        ReturnWeaponHeadToWeapon();
    }

    private void ReturnWeaponHeadToWeapon()
    {
        if (_weaponHeadRb.transform.position != _handleTransform.position && _throwState == ThrowState.Returning)
        {
            _timePassedReturning += Time.deltaTime;
            _weaponHeadRb.transform.position = Vector3.MoveTowards(_weaponHeadRb.transform.position, _handleTransform.position, Time.deltaTime * (_flyingSpeed / 10f) * _timePassedReturning);
            _bendPoint.position = Vector3.MoveTowards(_bendPoint.transform.position, _handleTransform.position, Time.deltaTime * (_flyingSpeed / 10f) * _timePassedReturning);
        }
        if (_weaponHeadRb.transform.position == _handleTransform.position && _throwState == ThrowState.Returning)
        {
            _weaponHeadRb.transform.SetParent(_handleTransform);
            _weaponHeadRb.transform.localPosition = _originalWeaponHeadPosition;
            _weaponHeadRb.transform.localRotation = _originalWeaponHeadRotation;
            _weapon.ShouldRotate = true;
            _throwState = ThrowState.Attached;
            _timePassedReturning = 0f;
            _bendPoint.position = _handleTransform.position;
        }
    }

    public virtual void ReturnWeaponHead()
    {
        _throwState = ThrowState.Returning;
    }

    private float CalculateAngle()
    {
        float x;
        float y;
        float h;
        float senA;
        float angle;

        x = _weaponHeadRb.transform.position.x - transform.position.x;
        y = _weaponHeadRb.transform.position.y - transform.position.y;

        h = Mathf.Sqrt(x * x + y * y);
        senA = y / h;
        angle = Mathf.Asin(senA);

        float degAngle = Mathf.Rad2Deg * angle;
        return degAngle;
    }

    private float CalculateAngleY()
    {
        if (_weaponHeadRb.transform.position.x < transform.position.x)
        {
            return 180;
        }
        else
        {
            return 0;
        }
    }

    #region Helper Classes

    public enum ThrowState
    {
        Attached,
        Throwing,
        Arrived,
        Stuck,
        GrabbingEnemy,
        Returning
    }

    #endregion
}