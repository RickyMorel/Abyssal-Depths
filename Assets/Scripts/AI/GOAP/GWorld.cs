using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResourceQueue
{
    public Queue<GameObject> Queue = new Queue<GameObject>();
    public string Tag;
    public string ModState;

    public ResourceQueue(string tag, string modState, WorldStates worldStates)
    {
        Tag = tag;
        ModState = modState;

        if(tag != "")
        {
            GameObject[] resources = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject resource in resources)
                Queue.Enqueue(resource);
        }

        if(modState != "")
        {
            worldStates.ModifyState(modState, Queue.Count);
        }
    }

    public void AddResource(GameObject resource)
    {
        Queue.Enqueue(resource);
    }

    public GameObject RemoveResource()
    {
        if(Queue.Count == 0) { return null; }

        return Queue.Dequeue();
    }
}

public sealed class GWorld : MonoBehaviour
{
    #region Private Variables

    private static GWorld _instance;

    private WorldStates _world;
    private ResourceQueue _eatingChairs;
    private ResourceQueue _shops;
    private ResourceQueue _hideLocations;
    private ResourceQueue _restLocations;
    private ResourceQueue _shipAttackPoints;
    private ResourceQueue _shipTurretAttackPoints;
    private ResourceQueue _healPoints;
    private ResourceQueue _rockPickupPoints;
    private ResourceQueue _megalodonRagdollPoints;
    private Dictionary<string, ResourceQueue> _resources = new Dictionary<string, ResourceQueue>();

    #endregion

    #region Public Properteis

    public static GWorld Instance => _instance;

    public static string FREE_EATINGCHAIR = "FreeEatingChair";
    public static string FREE_SHOPS = "FreeShops";
    public static string FREE_HIDE_LOCATIONS = "FreeHideLocation";
    public static string FREE_REST_LOCATIONS = "FreeRestLocation";
    public static string FREE_HEAL_POINTS = "FreeHealPoint";
    public static string FREE_ROCK_PICKUP_POINTS = "FreeRockPickupPoint";

    public static string EATINGCHAIRS = "eatingChairs";
    public static string SHOPS = "shops";
    public static string HIDE_LOCATIONS = "hideLocations";
    public static string REST_LOCATIONS = "restLocations";
    public static string HEAL_POINTS = "healPoints";
    public static string ROCK_PICKUP_POINTS = "rockPickupPoints";

    public enum AttackTags
    {
        shipAttackPoints = 1,
        shipTurretAttackPoints = 2,
        megalodonRagdollPoints = 3,
    }

    public enum AttackFreeTags
    {
        FreeShipAttackPoint = 1,
        FreeShipTurretAttackPoint = 2,
        FreeMegalodonRagdollPoint = 3,
    }

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
        StartCoroutine(LateStart());
    }

    //Calls InitializeWorld() later in order to prevent storing duplicate scene resources
    private IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();

        InitializeWorld();
    }

    private void InitializeWorld()
    {
        ClearOldData();

        _world = new WorldStates();
        _eatingChairs = new ResourceQueue("EatingChair", FREE_EATINGCHAIR, _world);
        _shops = new ResourceQueue("Shop", FREE_SHOPS, _world);
        _hideLocations = new ResourceQueue("HideLocation", FREE_HIDE_LOCATIONS, _world);
        _restLocations = new ResourceQueue("RestLocation", FREE_REST_LOCATIONS, _world);
        _shipAttackPoints = new ResourceQueue("ShipAttackPoint", AttackFreeTags.FreeShipAttackPoint.ToString(), _world);
        _shipTurretAttackPoints = new ResourceQueue("ShipTurretAttackPoint", AttackFreeTags.FreeShipTurretAttackPoint.ToString(), _world);
        _healPoints = new ResourceQueue("HealPoint", FREE_HEAL_POINTS, _world);
        _rockPickupPoints = new ResourceQueue("RockPickupPoint", FREE_ROCK_PICKUP_POINTS, _world);
        _megalodonRagdollPoints = new ResourceQueue("MegalodonRagdollPoint", AttackFreeTags.FreeMegalodonRagdollPoint.ToString(), _world);

        _resources.Add(EATINGCHAIRS, _eatingChairs);
        _resources.Add(SHOPS, _shops);
        _resources.Add(HIDE_LOCATIONS, _hideLocations);
        _resources.Add(REST_LOCATIONS, _restLocations);
        _resources.Add(AttackTags.shipAttackPoints.ToString(), _shipAttackPoints);
        _resources.Add(AttackTags.shipTurretAttackPoints.ToString(), _shipTurretAttackPoints);
        _resources.Add(HEAL_POINTS, _healPoints);
        _resources.Add(ROCK_PICKUP_POINTS, _rockPickupPoints);
        _resources.Add(AttackTags.megalodonRagdollPoints.ToString(), _megalodonRagdollPoints);
    }

    private void ClearOldData()
    {
        _world = null;
        _eatingChairs = null;
        _shops = null;
        _hideLocations = null;
        _restLocations = null;
        _shipAttackPoints = null;
        _shipTurretAttackPoints = null;
        _healPoints = null;
        _rockPickupPoints = null;
        _megalodonRagdollPoints = null;
        _resources.Clear();
    }

    public ResourceQueue GetQueue(string type)
    {
        return _resources[type];
    }

    private GWorld()
    {

    }

    public WorldStates GetWorld()
    {
        return _world;
    }
}
