using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveToggle : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject _collosal = null;
    [SerializeField] private GameObject[] _everythingElse = null;

    #endregion

    #region Private Variables

    private CameraManager _cameraManager;

    #endregion

    #region Unity Loops

    private void Start()
    {
        _cameraManager = FindObjectOfType<CameraManager>();

        StartCoroutine(OthoPerspectiveToggle(false));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Ship>() == null) { return; }

        StartCoroutine(OthoPerspectiveToggle(true));
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Ship>() == null) { return; }


        StartCoroutine(OthoPerspectiveToggle(false));
    }

    #endregion

    IEnumerator OthoPerspectiveToggle(bool boolean)
    {
        TimelinesManager.Instance.CameraFadeTimeline.Play();
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < _everythingElse.Length; i++)
        {
            _everythingElse[i].SetActive(boolean);
        }

        _collosal.SetActive(!boolean);
        _cameraManager.CullingMaskToggle(boolean);
    }
}