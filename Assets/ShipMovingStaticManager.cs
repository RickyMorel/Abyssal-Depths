using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipMovingStaticManager : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject ShipMovingObj;
    [SerializeField] private GameObject ShipStaticObj;
    [SerializeField] private GameObject ShipRenderCanvas;

    [Header("Cameras")]
    [SerializeField] private GameObject _staticShipCam;

    #endregion

    #region Private Variables

    private static ShipMovingStaticManager _instance;
    private Rigidbody _rb;
    private bool _isInGarage;

    #endregion

    #region Public Properties

    public static ShipMovingStaticManager Instance { get { return _instance; } }

    #endregion

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

    private void Start()
    {
        SceneManager.sceneLoaded += HandlSceneChange;

        _rb = ShipMovingObj.GetComponent<Rigidbody>();

        //HandlSceneChange(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        HandlSceneChange(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(EnableStaticShipCamDelayed());
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= HandlSceneChange;
    }

    private void HandlSceneChange(Scene scene, LoadSceneMode arg1)
    {
        SetShipState(scene.name == "SpaceStations");
    }

    public void SetShipState(bool isInGarage)
    {

        _rb.isKinematic = isInGarage;
        _rb.useGravity = !isInGarage;

        ShipRenderCanvas.SetActive(!isInGarage);

        //ShipStaticObj.isStatic = false;
        ShipStaticObj.transform.localPosition = isInGarage ? Vector3.zero : new Vector3(0f, -500f, 0f);
        //ShipStaticObj.isStatic = true;

        _staticShipCam.SetActive(!isInGarage);

        if (!isInGarage) { StartCoroutine(EnableStaticShipCamDelayed()); }
        else 
        {
            ShipMovingObj.transform.parent = transform;
            ShipMovingObj.transform.localPosition = Vector3.zero;
        }

        StartCoroutine(TeleportPlayersDelayed());

        StartCoroutine(ToggleCameraDelayed(isInGarage));

        _isInGarage = isInGarage;
    }

    private IEnumerator TeleportPlayersDelayed()
    {
        yield return new WaitForEndOfFrame();

        PlayerStateMachine[] players = FindObjectsOfType<PlayerStateMachine>();

        foreach (PlayerStateMachine player in players)
        {
            player.Teleport(ShipStaticObj.transform.position);
            player.InteractionController.CheckExitInteraction();
        }
    }

    private IEnumerator ToggleCameraDelayed(bool isInGarage)
    {
        yield return new WaitForSeconds(1f);

        CameraManager.Instance.ToggleCamera(!isInGarage);
    }

    private IEnumerator EnableStaticShipCamDelayed()
    {
        Debug.Log("EnableStaticShipCamDelayed");
        _staticShipCam.SetActive(false);

        yield return new WaitForEndOfFrame();

        _staticShipCam.SetActive(true);
    }
}
