using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "MainShip") { return; }

        GameManager.Instance.DeathManager.IsInSafeZone = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "MainShip") { return; }

        GameManager.Instance.DeathManager.IsInSafeZone = false;
    }
}
