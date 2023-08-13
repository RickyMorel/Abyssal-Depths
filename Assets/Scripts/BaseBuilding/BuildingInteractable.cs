using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class BuildingInteractable : MonoBehaviour
{
    #region Private Varaibles

    private Outline _outline;

    #endregion

    #region Public Properties

    public bool IsNear => _outline.enabled;

    public event Action OnInteract;
    public event Action OnUninteract;

    #endregion

    #region Unity Loops

    private void Start()
    {
        _outline = GetComponent<Outline>();

        _outline.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject != Ship.Instance.gameObject) { return; }

        _outline.enabled = true;

        Ship.Instance.InteractionController.SetCurrentInteractable(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject != Ship.Instance.gameObject) { return; }

        _outline.enabled = false;

        Uninteract();
    }

    #endregion

    public void Interact()
    {
        Debug.Log("Ship Interacted!");

        OnInteract?.Invoke();
    }

    public void Uninteract()
    {
        Ship.Instance.InteractionController.SetCurrentInteractable(null);
    }
}
