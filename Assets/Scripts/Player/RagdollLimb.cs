using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
//This class is used to stop the ragdoll from flying away
public class RagdollLimb : MonoBehaviour
{
    #region Private Variables

    private float _maxLimbVelocity = 10f;
    private Rigidbody _rb;

    #endregion

    #region Unity Loops

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _rb.velocity = new Vector3(
            Mathf.Clamp(_rb.velocity.x, -_maxLimbVelocity, _maxLimbVelocity),
            Mathf.Clamp(_rb.velocity.y, -_maxLimbVelocity, _maxLimbVelocity),
            Mathf.Clamp(_rb.velocity.z, -_maxLimbVelocity, _maxLimbVelocity)
            );
    }

    #endregion
}
