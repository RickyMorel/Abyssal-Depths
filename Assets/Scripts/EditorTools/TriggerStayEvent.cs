using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Separated OnTriggerStay from "TriggerEvents" because calling every frame is expensive
public class TriggerStayEvent : MonoBehaviour
{
    #region Public Properties

    public UnityEvent<Collider> OnTriggerStayEvent;

    #endregion

    private void OnTriggerStay(Collider other)
    {
        OnTriggerStayEvent?.Invoke(other);   
    }
}
