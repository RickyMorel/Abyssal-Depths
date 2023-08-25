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

    public virtual void Start()
    {
        _outline = GetComponent<Outline>();

        _outline.enabled = false;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if(other.gameObject != Ship.Instance.gameObject) { return; }

        _outline.enabled = true;

        Ship.Instance.InteractionController.SetCurrentInteractable(this);
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if(other.gameObject != Ship.Instance.gameObject) { return; }

        _outline.enabled = false;

        Uninteract();
    }

    #endregion

    public virtual bool CanUse()
    {
        return true;
    }

    public virtual void Interact()
    {
        OnInteract?.Invoke();
    }

    public virtual void Uninteract()
    {
        Ship.Instance.InteractionController.SetCurrentInteractable(null);
    }
}
