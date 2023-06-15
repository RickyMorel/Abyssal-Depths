using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelTriggerZone : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private BoxCollider _nextLevelTriggerZone;
    [SerializeField] private float _portalSuctionSpeed = 5;

    #endregion

    #region Private Variables

    private Booster _booster;
    private Ship _ship;

    #endregion

    #region Unity Loops

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Shield shield)) { return; }
        Debug.Log(other);
        if (_booster == null) { shield.transform.parent.GetComponentInChildren<Booster>(); }

        if (_ship == null) { shield.transform.parent.GetComponentInChildren<Ship>(); }
        Debug.Log(_booster);
        if (_booster.CanUse != false) { _booster.CanUse = false; }

        _ship.AddForceToShip((transform.position - _ship.transform.position).normalized * _portalSuctionSpeed, ForceMode.Force);
    }

    #endregion
}