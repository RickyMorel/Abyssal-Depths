using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Ship>() == null) { return; }

        ShipCamera.Instance.ChangeToBossZoom(true);

        Debug.Log("Entered Boss Zone");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Ship>() == null) { return; }

        ShipCamera.Instance.ChangeToBossZoom(false);

        Debug.Log("Exited Boss Zone");
    }
}
