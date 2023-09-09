using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class MovableRock : MonoBehaviour
{
    #region Private Variables

    private Drill _drill;

    #endregion

    #region Unity Loops

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.GetComponent<Drill>()) { return; }

        _drill = other.gameObject.GetComponent<Drill>();

        transform.SetParent(_drill.transform);
        Debug.Log("LOL");
        _drill.OnDestroyCurrentRock += DestroyThisRock;
    }

    #endregion

    private void DestroyThisRock()
    {
        Debug.Log("Hakai");
        Destroy(gameObject);
    }
}