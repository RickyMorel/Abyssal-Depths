using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableRock : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.GetComponent<Drill>()) { return; }

        transform.SetParent(other.transform);
    }
}