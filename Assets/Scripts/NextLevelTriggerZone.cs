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

    private Ship _ship = null;

    #endregion

    #region Unity Loops

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Hola");
        if (!other.gameObject.TryGetComponent(out Shader ship)) { Debug.Log("1"); return; }

        if (booster.CanUse != false) { booster.CanUse = false; }

        if (_ship == null) { _ship = booster.GetComponent<Ship>(); }
        Debug.Log("2");
        _ship.AddForceToShip((transform.position - _ship.transform.position).normalized * _portalSuctionSpeed, ForceMode.Force);
    }

    #endregion
}
