using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]  
public class BossZone : MonoBehaviour
{
    #region Editor Fields

    [Header("Boss")]
    [SerializeField] private GameObject _bossObject;

    [Header("Door")]
    [SerializeField] private Collider _bossDoorCollider;
    [SerializeField] private ParticleSystem _bossDoorParticles;

    #endregion

    #region Private Variables

    private Collider _collider;

    #endregion

    #region Unity Loops

    private void Awake()
    {
        _collider = GetComponent<Collider>();

        //Prevents calling OnTriggerEnter right after start
        _collider.enabled = false;
    }

    private void Start()
    {
        AIHealth.OnBossDied += HandleBossDied;

        _bossObject.SetActive(false);
        EnableDoor(false);

        StartCoroutine(LateStart());
    }

    private void OnDestroy()
    {
        AIHealth.OnBossDied -= HandleBossDied;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Ship>() == null) { return; }

        _bossObject.SetActive(true);
        EnableDoor(true);
    }

    #endregion

    private IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();

        _collider.enabled = true;
    }

    private void HandleBossDied()
    {
        EnableDoor(false);
    }

    private void EnableDoor(bool isEnabled)
    {
        if (isEnabled) _bossDoorParticles.Play(); else _bossDoorParticles.Stop();

        _bossDoorCollider.enabled = isEnabled;

        ShipCamera.Instance.BossCamera.gameObject.SetActive(isEnabled);
    }
}
