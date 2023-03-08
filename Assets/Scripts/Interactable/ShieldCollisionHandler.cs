using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCollisionHandler : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Projectile>() == null) { return; }
        
        Destroy(other.gameObject);
    }
}