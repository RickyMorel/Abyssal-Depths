using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipMovingStaticManager : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject _shipMovingObj;
    [SerializeField] private GameObject _shipStaticObj;
    [SerializeField] private GameObject _shipRenderCanvas;

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
    public GameObject ShipMovingObj => _shipMovingObj;
    public GameObject ShipStaticObj => _shipStaticObj;

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
        SetShipParentToCorrectStartingPosition();

        SceneManager.sceneLoaded += HandlSceneChange;

        _rb = _shipMovingObj.GetComponent<Rigidbody>();

        HandlSceneChange(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= HandlSceneChange;
    }

    private void HandlSceneChange(Scene scene, LoadSceneMode arg1)
    {
        SetShipState(scene.name == "SpaceStations", true);
    }

    private void SetShipParentToCorrectStartingPosition()
    {
#if UNITY_EDITOR
        if (!Ship.Instance.ShipData.LoadData) { return; }
#endif
        //Always set ShipParent position to Original SpaceStation location, it prevents sceneloading bugs
        transform.position = new Vector3(151.6f, -328.8f, 0f);
    }

    public void SetShipState(bool isInGarage, bool shouldTeleport)
    {

        _rb.isKinematic = isInGarage;
        _rb.useGravity = !isInGarage;

        _shipRenderCanvas.SetActive(!isInGarage);

        _shipStaticObj.transform.localPosition = isInGarage ? Vector3.zero : new Vector3(0f, -500f, 0f);

        _staticShipCam.SetActive(!isInGarage);

        if (!isInGarage) { StartCoroutine(EnableStaticShipCamDelayed()); }
        else 
        {
            _shipMovingObj.transform.parent = transform;
            _shipMovingObj.transform.localPosition = Vector3.zero;

            StartCoroutine(DestroyAllFixParts());
        }

        StartCoroutine(TeleportPlayersDelayed(shouldTeleport));

        //Heal everything when enter garage
        if (isInGarage) { Ship.Instance.ShipHealth.Respawn(); }

        _isInGarage = isInGarage;
    }

    private IEnumerator DestroyAllFixParts()
    {
        yield return new WaitForEndOfFrame();

        PlayerUpgradesController[] players = FindObjectsOfType<PlayerUpgradesController>();

        foreach (PlayerUpgradesController player in players)
        {
            player.DestroyHeldFixPart();
        }
    }

    private IEnumerator TeleportPlayersDelayed(bool shouldTeleport)
    {
        if (!shouldTeleport) { yield break; }

        yield return new WaitForEndOfFrame();

        PlayerStateMachine[] players = FindObjectsOfType<PlayerStateMachine>();

        foreach (PlayerStateMachine player in players)
        {
            player.Teleport(_shipStaticObj.transform.position);
            player.InteractionController.CheckExitInteraction();
        }
    }

    private IEnumerator EnableStaticShipCamDelayed()
    {
        _staticShipCam.SetActive(false);

        yield return new WaitForEndOfFrame();

        _staticShipCam.SetActive(true);
    }
}
