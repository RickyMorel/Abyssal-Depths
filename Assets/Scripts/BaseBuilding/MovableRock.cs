using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class MovableRock : MonoBehaviour
{
    #region Unity Loops

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.GetComponent<Drill>()) { return; }

        transform.SetParent(other.transform);
    }

    private void Start()
    {
        DayNightManager.Instance.OnCycleChange += DestroyRock;
    }

    private void OnDisable()
    {
        DayNightManager.Instance.OnCycleChange -= DestroyRock;
    }

    #endregion

    private void DestroyRock()
    {
        if (!transform.parent.GetComponent<Drill>()) { return; }

        Destroy(this);
    }
}