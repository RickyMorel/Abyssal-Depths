using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableRock : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("?");
        if (!other.gameObject.GetComponent<Drill>()) { return; }

        this.transform.SetParent(other.transform);
    }
}