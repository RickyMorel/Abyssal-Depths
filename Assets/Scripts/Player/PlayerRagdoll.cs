using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerRagdoll : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private List<Collider> _colliders = new List<Collider>();

    #endregion

    #region Private Variables

    private Collider _mainCollider;
    private Animator _anim;
    private NavMeshAgent _agent;

    #endregion

    #region Unity Loops

    private void Awake()
    {
        _mainCollider = GetComponent<Collider>();
        _anim = GetComponent<Animator>();

        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        LockZPos();
        EnableRagdoll(false);
    }

    #endregion

    private void LockZPos()
    {
        foreach (Collider collider in _colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezePositionZ;
        }
    }

    public void EnableRagdoll(bool isEnabled)
    {
        if (_colliders.Count < 1) { if (isEnabled) { PlayDeathAnimation(); } return; }

        if (isEnabled == false)
        {
            transform.position = _colliders[0].transform.position;
        }

        DisableMovement(isEnabled, true);

        foreach (Collider collider in _colliders)
        {
            collider.enabled = isEnabled;
        }
    }

    private void DisableMovement(bool isEnabled, bool hasRagdoll)
    {
        _mainCollider.enabled = !isEnabled;
        if (hasRagdoll) { _anim.enabled = !isEnabled; }
        if (_agent != null) { _agent.enabled = !isEnabled; }
    }

    private void PlayDeathAnimation()
    {
        _anim.SetBool("IsDead", true);
        DisableMovement(true, false);
    }
}
