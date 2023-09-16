using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent (typeof(BoxCollider))]
public class BiomeZone : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private DayNightSO _biomeDayNightSO;

    #endregion

    #region Private Variables

    private bool _isInBiome = false;

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject != Ship.Instance.gameObject) { return; }

        Debug.Log("OnTriggerEnter: Biome");

        _isInBiome = true;

        DayNightManager.Instance.BrightnessLerpByBiome(_biomeDayNightSO);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != Ship.Instance.gameObject) { return; }

        _isInBiome = false;

        DayNightManager.Instance.ResetBiome();
    }
}
