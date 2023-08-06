using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;
using System;
using UnityEngine.Analytics;

public class CameraManager : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private List<CinemachineBrain> _cameras = new List<CinemachineBrain>();
    [SerializeField] private List<CinemachineVirtualCamera> _vCams = new List<CinemachineVirtualCamera>();

    #endregion

    #region Private Variables

    private static CameraManager _instance;
    private bool _isInOrthoMode = false;

    #endregion

    #region Public Properties

    public static CameraManager Instance { get { return _instance; } }
    public bool IsInOrthoMode => _isInOrthoMode;

    #endregion

    #region Unity Loops

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion

    private void GetAllCameras()
    {
        _cameras.Clear();
        _vCams.Clear();

        _cameras = FindObjectsOfType<CinemachineBrain>(true).ToList();
        _vCams = FindObjectsOfType<CinemachineVirtualCamera>(true).ToList();

        _cameras.Sort((a, b) => String.Compare(LayerMask.LayerToName(a.gameObject.layer), LayerMask.LayerToName(b.gameObject.layer)));
        _vCams.Sort((a, b) => String.Compare(LayerMask.LayerToName(a.gameObject.layer), LayerMask.LayerToName(b.gameObject.layer)));

        for (int i = 0; i < _vCams.Count; i++)
        {
            if (_vCams[i].Name.Contains("0-"))
            {
                var swappedCamera = _vCams[i];
                _vCams[i] = _vCams[_vCams.Count - 1];
                _vCams[_vCams.Count - 1] = swappedCamera;
            }
            else if (_vCams[i].Name.Contains("Y-"))
            {
                var swappedCamera = _vCams[i];
                _vCams[i] = _vCams[0];
                _vCams[0] = swappedCamera;
            }
        }
    }

    public void ToggleCamera(bool boolean)
    {
        //Stops previous camera toggle functions from calling
        GetAllCameras();

        ShipCamera.Instance.PerspectiveCamera.gameObject.SetActive(boolean);
        ShipCamera.Instance.EnemyFocusVCam.enabled = boolean;
        ShipCamera.Instance.EnemyFocusVCam.gameObject.SetActive(boolean);
        ShipCamera.Instance.gameObject.SetActive(boolean);

        for (int i = 0; i < _cameras.Count; i++)
        {
            if (_vCams[i].gameObject != ShipCamera.Instance.gameObject) { _vCams[i].gameObject.SetActive(!boolean); }
            if (_cameras[i].tag != "MainCamera") {  _cameras[i].gameObject.SetActive(!boolean); }
        }

        _isInOrthoMode = boolean;
    }

    public void SplitScreen(int index)
    {
        GetAllCameras();
        ToggleCamera(false);
        if (index == 0)
        {
            _vCams[1].gameObject.layer = 29;
            //Turn On Layer
            _cameras[2].OutputCamera.cullingMask |= 1 << LayerMask.NameToLayer("Player2Cam");
            //Turn Off Layer
            _cameras[2].OutputCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Player1Cam"));
            EnablePlayerCamera(2);

            _cameras[2].OutputCamera.rect = new Rect(0, 0.5f, 1, 0.5f);
            _cameras[1].OutputCamera.rect = new Rect(0, 0f, 1, 0.5f);
        }
        else if (index == 1)
        {
            _vCams[1].gameObject.layer = 30;
            _cameras[3].OutputCamera.cullingMask |= 1 << LayerMask.NameToLayer("Player3Cam");
            _cameras[3].OutputCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Player1Cam"));
            EnablePlayerCamera(3);

            _cameras[3].OutputCamera.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
            _cameras[2].OutputCamera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            _cameras[1].OutputCamera.rect = new Rect(0, 0, 1, 0.5f);
        }
        else if (index == 2)
        {
            _vCams[1].gameObject.layer = 31;
            _cameras[4].OutputCamera.cullingMask |= 1 << LayerMask.NameToLayer("Player4Cam");
            _cameras[4].OutputCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Player1Cam"));
            EnablePlayerCamera(4);

            _cameras[4].OutputCamera.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
            _cameras[3].OutputCamera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            _cameras[2].OutputCamera.rect = new Rect(0, 0, 0.5f, 0.5f);
            _cameras[1].OutputCamera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
        }
    }

    private void EnablePlayerCamera(int cameraIndex)
    {
        _cameras[cameraIndex].gameObject.SetActive(true);
        _vCams[cameraIndex].gameObject.SetActive(true);
    }
}