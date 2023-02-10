using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.GetComponent<Ship>()) { return; }

        GameManager.Instance.DeathManager.IsInSafeZone = true;

        ShipInventory.Instance.TransferAllItemsToNewInventory(MainInventory.Instance);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.GetComponent<Ship>()) { return; }

        GameManager.Instance.DeathManager.IsInSafeZone = false;
    }
}
