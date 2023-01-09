using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;
using System;

public class CameraManager : MonoBehaviour
{
    #region Private Variables

    private static CameraManager _instance;

    private CinemachineBrain[] _cameras;
    private CinemachineVirtualCamera[] _vCams;
    
    private GameObject _perspectiveCamera;
    private bool _isInOrthoMode = false;

    #endregion

    #region Public Properties

    public static CameraManager Instance { get { return _instance; } }
    public bool IsInOrthoMode => _isInOrthoMode;

    #endregion

    #region Unity Loops

    private void Start()
    {
        GetAllCameras();
        _perspectiveCamera = _cameras[_cameras.Length - 1].transform.GetChild(0).gameObject;
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
            _perspectiveCamera.GetComponent<Camera>().cullingMask = LayerMask.GetMask("Floor");
        }
        else
        {
            _cameras[_cameras.Length - 1].OutputCamera.cullingMask = LayerMask.GetMask("Ragdoll", "ShipFloor", "Orthographic", "UI");
            _perspectiveCamera.GetComponent<Camera>().cullingMask = LayerMask.GetMask("Floor", "Default", "TransparentFX", "Ignore Raycast", "Water", "UI", "LootLayer", "Cylinder", "ItemLayer", "ItemBox", "NPC", "EnemyHitBox");
        }
    }

    private void GetAllCameras()
    {
        _cameras = FindObjectsOfType<CinemachineBrain>(true);
        _vCams = FindObjectsOfType<CinemachineVirtualCamera>(true);

        Array.Sort(_cameras, (a, b) => String.Compare(a.name, b.name));
        Array.Sort(_vCams, (a, b) => String.Compare(a.name, b.name));
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

        _isInOrthoMode = boolean;
    }

    public void SplitScreen(int index)
    {
        GetAllCameras();
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