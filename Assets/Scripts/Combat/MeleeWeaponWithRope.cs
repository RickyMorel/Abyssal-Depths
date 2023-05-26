using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponWithRope : MeleeWeapon
{
    #region Editor Fields

    [SerializeField] private Transform _handleTransform;
    [SerializeField] private Rigidbody _weaponHeadRb;
    [SerializeField] private Transform _bendPoint;

    #endregion

    #region Private Variables

    private Transform _middlePointTransform;

    #endregion

    #region Unity Loops

    public virtual void LateUpdate()
    {
        DrawRope();
    }

    #endregion

    private void DrawRope()
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
}