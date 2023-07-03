using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class ShipCamera : BaseCamera
{
    #region Editor Fields

    [SerializeField] private Camera _perspectiveCamera;
    [SerializeField] private CinemachineVirtualCamera _enemyFocusVCam;
    [SerializeField] private Transform _cameraLookAtTransform;
    [SerializeField] private ZoomValues[] _zoomDistancesArray;
    [SerializeField] private float _maxEnemyFocusDistance = 30f;

    #endregion

    #region Private Variables

    private static ShipCamera _instance;
    private Rigidbody _shipRigidbody;
    private Booster _shipBooster;

    private bool _isBoosting;
    private float _currentFOV;
    private float _orginalFOV;
    private float _targetPerspectiveZ;
    private int _currentZoomDistanceIndex = 1;

    private GameObject _enemyLookObject;
    private Transform _currentEnemy;
    #endregion

    #region Public Properties

    public static ShipCamera Instance { get { return _instance; } }
    public Camera PerspectiveCamera => _perspectiveCamera;
    public CinemachineVirtualCamera EnemyFocusVCam => _enemyFocusVCam;

    #endregion

    #region Unity Loops

    private void Awake()
    {
        Booster.OnBoostUpdated += HandleBoost;
        SceneManager.sceneLoaded += HandleSceneLoaded;

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public override void Start()
    {
        base.Start();

        _shipRigidbody = FindObjectOfType<ShipHealth>().GetComponent<Rigidbody>();
        _shipBooster = _shipRigidbody.transform.root.GetComponentInChildren<Booster>();

        PlayerInputHandler.OnChangeZoom += HandleChangeZoom;

        _orginalFOV = _virtualCamera.m_Lens.FieldOfView;
        _currentFOV = _orginalFOV;

        _enemyLookObject = new GameObject("EnemyCameraLookTransform");
        _enemyFocusVCam.Follow = _enemyLookObject.transform;

        StartCoroutine(LateStart());

        ChangeZoom();
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();

        _enemyFocusVCam.gameObject.SetActive(true);
    }

    //Only player that is driving can change the zoom
    private void HandleChangeZoom(PlayerInputHandler requestingPlayer)
    {
        if(_shipBooster.CurrentPlayer == null) { return; }

        if(requestingPlayer != _shipBooster.CurrentPlayer.PlayerInput) { return; }

        ChangeZoom();
    }

    private void FixedUpdate()
    {
        UpdateBoostFOVEffect();
        UpdateCameraZoom();
        LookAtEnemy();
    }

    private void OnDestroy()
    {
        Booster.OnBoostUpdated -= HandleBoost;
        PlayerInputHandler.OnChangeZoom -= HandleChangeZoom;
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    #endregion

    private void HandleSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        _enemyLookObject = new GameObject("EnemyCameraLookTransform");
        _enemyFocusVCam.Follow = _enemyLookObject.transform;
    }

    private void LookAtEnemy()
    {
        if (_currentEnemy != null && Vector3.Distance(_currentEnemy.position, Ship.Instance.transform.position) < _maxEnemyFocusDistance)
        {
            Vector3 halfPoint = (_currentEnemy.position + _cameraLookAtTransform.transform.position) / 2f;
            Vector3 quarterPoint = (halfPoint + _cameraLookAtTransform.transform.position) / 2f;

            _enemyLookObject.transform.position = quarterPoint;
            _enemyLookObject.transform.position = new Vector3(
                _enemyLookObject.transform.position.x,
                _enemyLookObject.transform.position.y,
                _cameraLookAtTransform.position.z
                );
            return;
        }

        _currentEnemy = FindClosestEnemy();

        if(_currentEnemy == null) { _enemyFocusVCam.Priority = 0; return; }

        _enemyFocusVCam.Priority = _virtualCamera.Priority + 1;
    }

    private Transform FindClosestEnemy()
    {
        AIHealth[] enemies = FindObjectsOfType<AIHealth>();

        foreach (AIHealth enemy in enemies)
        {
            if(Vector3.Distance(enemy.transform.position, Ship.Instance.transform.position) > _maxEnemyFocusDistance) { continue; }

            return enemy.transform;
        }

        return null;
    }

    private void UpdateCameraZoom()
    {
        float zoomSpeed = 5f;
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, new Vector3(0f, 0f, _targetPerspectiveZ), Time.deltaTime * zoomSpeed);
        _cameraLookAtTransform.localPosition = Vector3.MoveTowards(_cameraLookAtTransform.localPosition, new Vector3(0f, 0f, _targetPerspectiveZ), Time.deltaTime * zoomSpeed);
    }

    private void UpdateBoostFOVEffect()
    {
        float expandedBoostFOV = _orginalFOV + (_shipRigidbody.velocity.magnitude * _expandedFovToVelocityRatio);
        float wantedFOV = _isBoosting == true && _shipBooster.RecentlyChangedGear == false ? expandedBoostFOV : _orginalFOV;

        _currentFOV = Mathf.Lerp(_currentFOV, wantedFOV, Time.deltaTime);
        _virtualCamera.m_Lens.FieldOfView = _currentFOV;
    }

    private void HandleBoost(bool isBoosting)
    {
        _isBoosting = isBoosting;
    }

    public void ChangeZoom()
    {
        _currentZoomDistanceIndex++;

        if(_currentZoomDistanceIndex > _zoomDistancesArray.Length - 1) { _currentZoomDistanceIndex = 0; }

        _targetPerspectiveZ = _zoomDistancesArray[_currentZoomDistanceIndex].Distance;
        _orginalFOV = _zoomDistancesArray[_currentZoomDistanceIndex].FOV;
    }

    [System.Serializable]
    public class ZoomValues
    {
        public float Distance;
        public float FOV;
        public ZoomValues(float distance, float fov)
        {
            this.Distance = distance;
            this.FOV = fov;
        }
    }
}