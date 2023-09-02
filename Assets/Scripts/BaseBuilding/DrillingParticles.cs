using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillingParticles : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.GetComponent<Drill>()) { return; }

        GetComponent<ParticleSystem>().Play();
    }
}