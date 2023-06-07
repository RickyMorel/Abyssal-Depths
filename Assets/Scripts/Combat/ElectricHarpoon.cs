using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class ElectricHarpoon : MeleeWeaponWithRope
{
    #region Editor Fields

    [Header("Transforms")]
    [SerializeField] private Collider _harpoonCollider;
    [Header("Floats")]
    [SerializeField] private float _grappleSpeed;
    [SerializeField] private float _maxElectricBeaconDistance;
    [Header("GameObject Related")]
    [SerializeField] private Collider _trackEnemiesZone;
    [SerializeField] private GameObject _electructionZonePrefab;
    [SerializeField] private GameObject _electricWireBeaconPrefab;

    
    #endregion

    #region Private Variables

    private bool _prevInput2State;
    private SpringJoint _tetherSpringInstance;
    private ElectricZoneInstanceClass _electrocutionZoneInstance;
    private AIStateMachine _tetheredEnemy;
    private List<GameObject> _electricWireBeacons = new List<GameObject>();
    private float _timeSincePressInput;

    #endregion

    #region Unity Loops

    public override void Start()
    {
        base.Start();

        _electrocutionZoneInstance = new ElectricZoneInstanceClass(Instantiate(_electructionZonePrefab));
        _electrocutionZoneInstance.AttackHitBox.Initialize(_weapon, _weapon.InteractableHealth, this);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        _weaponHeadRb.isKinematic = _throwState == ThrowState.Attached || _throwState == ThrowState.Returning || _throwState == ThrowState.Stuck || _throwState == ThrowState.GrabbingEnemy;
        //set's harpoon collider to trigger when grabbing enemy to prevent weird physics collisions
        _harpoonCollider.isTrigger = _throwState == ThrowState.GrabbingEnemy;


        if (_throwState == ThrowState.GrabbingEnemy && _tetheredEnemy != null) { _weaponHeadRb.transform.position = _tetheredEnemy.transform.position; }

        ThrowWeaponHead();
    }

    public override void LateUpdate()
    {
        _timeSincePressInput += Time.deltaTime;

        base.LateUpdate();

        //Update line positions every frame
        if (_electricWireBeacons.Count == 2) { SetupElectricWire(); }
    }

    //Invoked by UnityEvent
    public void OnCollide(Collider collider)
    {
        if (_throwState == ThrowState.Attached) { return; }
        if (_throwState == ThrowState.Returning) { return; }
        if (_throwState == ThrowState.GrabbingEnemy) { return; }

        if (collider.transform.root.tag == "MainShip") { return; }

        _throwState = ThrowState.Stuck;

        if (collider.transform.TryGetComponent(out AIStateMachine aIState)) { _throwState = ThrowState.GrabbingEnemy; aIState.SetRagdollState(true); }

        CreateSpringObject(aIState);
    }

    #endregion

    #region Shoot Functions

    public override void CheckShootInput()
    {
        //Grapple ship towards harpoon
        if (_weapon.CurrentPlayer.IsUsing_2 && _throwState == ThrowState.Stuck)
        {
            Vector3 grappleDirection = _weaponHeadRb.transform.position - Ship.Instance.transform.position;
            Vector3 finalForce = grappleDirection.normalized * _grappleSpeed * Time.deltaTime;
            Ship.Instance.AddForceToShip(finalForce, ForceMode.Force);
            return;
        }

        CheckShootBeacons();

        //Shoots harpoon
        if (_weapon.CurrentPlayer.IsUsing == _prevInputState) { return; }

        _prevInputState = _weapon.CurrentPlayer.IsUsing;

        if (_weapon.CurrentPlayer.IsUsing)
        {
            Shoot();
        }
    }

    private void CheckShootBeacons()
    {
        if (_weapon.CurrentPlayer.PlayerInput.DetectDoubleTap())
        {
            DestroyBeacons();
            return;
        }

        if(_timeSincePressInput < 0.4f) { return; }

        if (_weapon.CurrentPlayer.IsUsing_2 == _prevInput2State) { return; }

        _prevInput2State = _weapon.CurrentPlayer.IsUsing_2;

        _timeSincePressInput = 0f;

        //Shoot electric wire beacons
        if (_weapon.CurrentPlayer.IsUsing_2 && _throwState != ThrowState.Stuck && _throwState != ThrowState.GrabbingEnemy)
        {
            TrySpawnWireOrSpawnBeacon();

            return;
        }
    }

    #endregion

    #region Electric Wire Functions

    private void TrySpawnWireOrSpawnBeacon()
    {
        //Destroy previous beacons
        if (_electricWireBeacons.Count >= 2) { DestroyBeacons(); }

        GameObject beaconInstance = Instantiate(_electricWireBeaconPrefab, _weaponHeadRb.transform.position, Quaternion.identity);
        Projectile beaconProjectileScript = beaconInstance.GetComponent<Projectile>();
        beaconProjectileScript.WeaponReference = _weapon;
        beaconProjectileScript.Launch(_weaponHeadRb.transform.up);

        _electricWireBeacons.Add(beaconInstance);

        if(_electricWireBeacons.Count > 1 && 
            Vector3.Distance(_electricWireBeacons[0].transform.position, _electricWireBeacons[1].transform.position) > _maxElectricBeaconDistance)
        {
            DestroyBeacons();
            TrySpawnWireOrSpawnBeacon();
        }

        SetupElectricWire();
    }

    private void DestroyBeacons()
    {
        foreach (GameObject beacon in _electricWireBeacons)
        {
            Destroy(beacon);
        }
        _electricWireBeacons.Clear();

        SetupElectricWire();
    }

    private void SetupElectricWire()
    {
        if(_electricWireBeacons.Count < 2) { _electrocutionZoneInstance.Lr.enabled = false; _electrocutionZoneInstance.ElectricZoneGameobject.SetActive(false); return; }

        _electrocutionZoneInstance.Lr.enabled = true;
        _electrocutionZoneInstance.ElectricZoneGameobject.SetActive(true);
        _electrocutionZoneInstance.Lr.SetPosition(0, _electricWireBeacons[0].transform.position);
        _electrocutionZoneInstance.Lr.SetPosition(1, _electricWireBeacons[1].transform.position);

        AdjustElectrocutionZone();
    }

    private void AdjustElectrocutionZone()
    {
        GameObject electricZoneObj = _electrocutionZoneInstance.ElectricZoneGameobject;

        electricZoneObj.transform.localScale =
            new Vector3(
                Vector3.Distance(_electricWireBeacons[0].transform.position, _electricWireBeacons[1].transform.position),
                electricZoneObj.transform.localScale.y,
                electricZoneObj.transform.localScale.z
                );

        electricZoneObj.transform.position =
            new Vector3(
                (_electricWireBeacons[0].transform.position.x + _electricWireBeacons[1].transform.position.x) / 2f,
                (_electricWireBeacons[0].transform.position.y + _electricWireBeacons[1].transform.position.y) / 2f,
                _handleTransform.transform.position.z
                );

        Vector3 direction = _electricWireBeacons[0].transform.position - _electricWireBeacons[1].transform.position;
        float angle = Vector3.Angle(direction, transform.up);

        //Determine if angle is positive
        Vector3 cross = Vector3.Cross(direction, transform.up);

        if (cross.z > 0)
        {
            angle = -angle;
        }

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

        electricZoneObj.transform.rotation = targetRotation;
    }

    #endregion

    #region Harpoon Functions

    public override void DrawRope()
    {
        if (_throwState == ThrowState.Returning || _throwState == ThrowState.Attached) { return; }

        base.DrawRope();
    }

    private void CreateSpringObject(AIStateMachine enemy = null)
    {
        Destroy(_tetherSpringInstance);

        if (_throwState != ThrowState.Stuck && _throwState != ThrowState.GrabbingEnemy) { return; }

        _tetherSpringInstance = Ship.Instance.gameObject.AddComponent<SpringJoint>();

        _tetherSpringInstance.connectedBody = enemy ? enemy.Rb : _weaponHeadRb;
        _tetherSpringInstance.maxDistance = Vector3.Distance(Ship.Instance.transform.position, _weaponHeadRb.transform.position);
        _tetherSpringInstance.massScale = Ship.Instance.Rb.mass;
        _tetherSpringInstance.connectedMassScale = enemy ? enemy.Rb.mass : 1f;
        _tetheredEnemy = enemy;
    }

    public override void ReturnWeaponHead()
    {
        base.ReturnWeaponHead();
        Destroy(_tetherSpringInstance);

        if(_tetheredEnemy == null) { return; }

        _tetheredEnemy.SetRagdollState(false);
        _tetheredEnemy = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(_weaponHeadRb.transform.position, Vector3.one * 2);
        Gizmos.DrawCube(_handleTransform.transform.position, Vector3.one * 2);
        Gizmos.DrawCube(_bendPoint.transform.position, Vector3.one * 2);
        Vector3 moveToForMiddleRope = new Vector3((_weaponHeadRb.transform.position.x + _handleTransform.position.x) / 2, (_weaponHeadRb.transform.position.y + _handleTransform.position.y) / 2, (_weaponHeadRb.transform.position.z + _handleTransform.position.z) / 2);
        Gizmos.DrawCube(moveToForMiddleRope, Vector3.one * 2);
    }

    #endregion

    #region Helper Classes

    private class ElectricZoneInstanceClass
    {
        public ElectricZoneInstanceClass(GameObject electricZoneInstance)
        {
            ElectricZoneGameobject = electricZoneInstance;
            AttackHitBox = electricZoneInstance.GetComponent<WeaponAttackHitBox>();
            Lr = electricZoneInstance.GetComponent<LineRenderer>();
        }

        public GameObject ElectricZoneGameobject;
        public WeaponAttackHitBox AttackHitBox;
        public LineRenderer Lr;
    }

    #endregion
}