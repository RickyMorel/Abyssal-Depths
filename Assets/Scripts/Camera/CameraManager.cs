using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

public class CameraManager : MonoBehaviour
{
    #region Private Variables

    private static CameraManager _instance;

    private CinemachineBrain[] _cameras;
    private CinemachineVirtualCamera[] _vCams;

    private GameObject _perspectiveCamera;

    #endregion

    #region Public Properties

    public static CameraManager Instance { get { return _instance; } }

    #endregion

    #region Unity Loops

    private void Start()
    {
        GetAllCameras();

        _perspectiveCamera = GameObject.Find("Perspective Camera");
    }

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

    public void CullingMaskToggle(bool boolean)
    {
        if (boolean)
        {
            _cameras[_cameras.Length - 1].OutputCamera.cullingMask = -1;
            _cameras[_cameras.Length - 1].gameObject.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
            _perspectiveCamera.SetActive(false);
        }
        else
        {
            _cameras[_cameras.Length - 1].OutputCamera.cullingMask = LayerMask.GetMask("Ragdoll", "ShipFloor", "Orthographic");
            _cameras[_cameras.Length - 1].gameObject.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
            _perspectiveCamera.SetActive(true);
        }
    }

    private void GetAllCameras()
    {
        _cameras = FindObjectsOfType<CinemachineBrain>(true);
        _cameras.OrderBy(p => p.name).ToList();
        _vCams = FindObjectsOfType<CinemachineVirtualCamera>(true);
        _vCams.OrderBy(p => p.name).ToList();
    }

    public void ToggleCamera(bool boolean)
    {
        GetAllCameras();

        _cameras[_cameras.Length - 1].gameObject.SetActive(boolean);
        _vCams[_vCams.Length - 1].gameObject.SetActive(boolean);

        for (int i = 0; i < _cameras.Length - 1; i++)
        {
            _cameras[i].gameObject.SetActive(!boolean);
            _vCams[i].gameObject.SetActive(!boolean);
        }
    }

    public void SplitScreen(int index)
    {
        _cameras = FindObjectsOfType<CinemachineBrain>(true);
        _cameras.OrderBy(p => p.name).ToList();
        _vCams = FindObjectsOfType<CinemachineVirtualCamera>(true);
        _vCams.OrderBy(p => p.name).ToList();
        ToggleCamera(false);
        if (index == 0)
        {
            _vCams[0].gameObject.layer = 29;
            _cameras[1].OutputCamera.rect = new Rect(0, 0.5f, 1, 0.5f);
            _cameras[0].OutputCamera.rect = new Rect(0, 0, 1, 0.5f);
        }
        else if (index == 1)
        {
            _vCams[0].gameObject.layer = 30;
            _cameras[2].OutputCamera.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
            _cameras[1].OutputCamera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            _cameras[0].OutputCamera.rect = new Rect(0, 0, 1, 0.5f);
        }
        else if (index == 2)
        {
            _vCams[0].gameObject.layer = 31;
            _cameras[3].OutputCamera.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
            _cameras[2].OutputCamera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            _cameras[1].OutputCamera.rect = new Rect(0, 0, 0.5f, 0.5f);
            _cameras[0].OutputCamera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
        }
    }
}