using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class ElectricHarpoon : MeleeWeapon
{
    #region Editor Fields

    [Header("Transforms")]
    [SerializeField] private Transform _moveToTransform;
    [SerializeField] private Transform _handleTransform;
    [SerializeField] private Rigidbody _harpoonRb;
    [SerializeField] private Collider _harpoonCollider;
    [SerializeField] private Transform _bendPoint;
    [Header("Floats")]
    [SerializeField] private float _flyingSpeed;
    [SerializeField] private float _grappleSpeed;
    [SerializeField] private float _maxElectricBeaconDistance;
    [Header("GameObject Related")]
    [SerializeField] private Collider _trackEnemiesZone;
    [SerializeField] private GameObject _electructionZonePrefab;
    [SerializeField] private GameObject _electricWireBeaconPrefab;

    
    #endregion

    #region Private Variables

    private Vector3 _originalHarpoonPosition;
    private Quaternion _originalHarpoonRotation;
    private ThrowState _throwState = ThrowState.Attached;
    private bool _prevInputState;
    private bool _prevInput2State;
    private float _timePassedReturning;
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

        _originalHarpoonPosition = _harpoonRb.transform.localPosition;
        _originalHarpoonRotation = _harpoonRb.transform.localRotation;
    }

    public override void Update()
    {
        base.Update();

        if(_harpoonRb.transform.parent != null)
        {
            _harpoonRb.transform.localRotation = Quaternion.Euler(0, 270, 270);
        }
        else
        {
            _harpoonRb.transform.rotation = Quaternion.Euler(0, CalculateAngleY(), CalculateAngleZ() - 90);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        _harpoonRb.isKinematic = _throwState == ThrowState.Attached || _throwState == ThrowState.Returning || _throwState == ThrowState.Stuck || _throwState == ThrowState.GrabbingEnemy;
        //set's harpoon collider to trigger when grabbing enemy to prevent weird physics collisions
        _harpoonCollider.isTrigger = _throwState == ThrowState.GrabbingEnemy;


        if (_throwState == ThrowState.GrabbingEnemy && _tetheredEnemy != null) { _harpoonRb.transform.position = _tetheredEnemy.transform.position; }

        ThrowHarpoon();
    }

    private void LateUpdate()
    {
        _timeSincePressInput += Time.deltaTime;

        DrawRope();

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

    private float CalculateAngleZ()
    {
        float x;
        float y;
        float h;
        float senA;
        float angle;

        x = _harpoonRb.transform.position.x - transform.position.x;
        y = _harpoonRb.transform.position.y - transform.position.y;

        h = Mathf.Sqrt(x * x + y * y);
        senA = y / h;
        angle = Mathf.Asin(senA);

        float degAngle = Mathf.Rad2Deg * angle;
        return degAngle;
    }

    private float CalculateAngleY()
    {
        if (_harpoonRb.transform.position.x < transform.position.x)
        {
            return 180;
        }
        else
        {
            return 0;
        }
    }

    #region Shoot Functions

    public override void CheckShootInput()
    {
        //Grapple ship towards harpoon
        if (_weapon.CurrentPlayer.IsUsing_2 && _throwState == ThrowState.Stuck)
        {
            Vector3 grappleDirection = _harpoonRb.transform.position - Ship.Instance.transform.position;
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

    public override void Shoot()
    {
        if (_throwState != ThrowState.Attached) 
        { 
            if(_throwState == ThrowState.Arrived || _throwState == ThrowState.Stuck || _throwState == ThrowState.GrabbingEnemy) { ReturnHarpoon(); }

            return; 
        }

        _throwState = ThrowState.Throwing;
        _harpoonRb.isKinematic = false;
        _weapon.ShouldRotate = false;

        _harpoonRb.transform.SetParent(null);
    }

    #endregion

    #region Electric Wire Functions

    private void TrySpawnWireOrSpawnBeacon()
    {
        //Destroy previous beacons
        if (_electricWireBeacons.Count >= 2) { DestroyBeacons(); }

        GameObject beaconInstance = Instantiate(_electricWireBeaconPrefab, _harpoonRb.transform.position, Quaternion.identity);
        Projectile beaconProjectileScript = beaconInstance.GetComponent<Projectile>();
        beaconProjectileScript.WeaponReference = _weapon;
        beaconProjectileScript.Launch(_harpoonRb.transform.up);

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

    private void ThrowHarpoon()
    {
        if (_throwState == ThrowState.Arrived) { return; }

        Transform moveToCurrentPosition = _moveToTransform;

        if (_throwState == ThrowState.Throwing)
        {
            Vector3 forceDir = _moveToTransform.position - _harpoonRb.transform.position;
            _harpoonRb.AddForce(forceDir.normalized * _flyingSpeed, ForceMode.Force);
        }

        if (_throwState == ThrowState.Throwing && Vector3.Distance(_harpoonRb.transform.position, moveToCurrentPosition.position) < 2f)
        {
            _throwState = ThrowState.Arrived;
        }

        ReturnHarpoonToWeapon();
    }

    private void ReturnHarpoonToWeapon()
    {
        if (_harpoonRb.transform.position != _handleTransform.position && _throwState == ThrowState.Returning)
        {
            _timePassedReturning += Time.deltaTime;
            _harpoonRb.transform.position = Vector3.MoveTowards(_harpoonRb.transform.position, _handleTransform.position, Time.deltaTime * (_flyingSpeed / 10f) * _timePassedReturning);
            _bendPoint.position = Vector3.MoveTowards(_bendPoint.transform.position, _handleTransform.position, Time.deltaTime * (_flyingSpeed / 10f) * _timePassedReturning);
        }
        if (_harpoonRb.transform.position == _handleTransform.position && _throwState == ThrowState.Returning)
        {
            _harpoonRb.transform.SetParent(_handleTransform);
            _harpoonRb.transform.localPosition = _originalHarpoonPosition;
            _harpoonRb.transform.localRotation = _originalHarpoonRotation;
            _weapon.ShouldRotate = true;
            _throwState = ThrowState.Attached;
            _timePassedReturning = 0f;
            _bendPoint.position = _handleTransform.position;
        }
    }

    private void DrawRope()
    {
        if (_throwState == ThrowState.Returning || _throwState == ThrowState.Attached) { return; }

        Vector3 moveToForMiddleRope = new Vector3((_harpoonRb.transform.position.x + _handleTransform.position.x) / 2, (_harpoonRb.transform.position.y + _handleTransform.position.y) / 2, (_harpoonRb.transform.position.z + _handleTransform.position.z) / 2);

        if (Vector3.Distance(_bendPoint.position, _handleTransform.position) > 15 || Vector3.Distance(_bendPoint.position, _harpoonRb.transform.position) > 15)
        {
            _bendPoint.position = _bendPoint.position;
        }
        else
        {
            _bendPoint.position = Vector3.MoveTowards(_bendPoint.position, moveToForMiddleRope, Time.deltaTime * 3);
        }
    }

    private void CreateSpringObject(AIStateMachine enemy = null)
    {
        Destroy(_tetherSpringInstance);

        if (_throwState != ThrowState.Stuck && _throwState != ThrowState.GrabbingEnemy) { return; }

        _tetherSpringInstance = Ship.Instance.gameObject.AddComponent<SpringJoint>();

        _tetherSpringInstance.connectedBody = enemy ? enemy.Rb : _harpoonRb;
        _tetherSpringInstance.maxDistance = Vector3.Distance(Ship.Instance.transform.position, _harpoonRb.transform.position);
        _tetherSpringInstance.massScale = Ship.Instance.Rb.mass;
        _tetherSpringInstance.connectedMassScale = enemy ? enemy.Rb.mass : 1f;
        _tetheredEnemy = enemy;
    }

    private void ReturnHarpoon()
    {
        _throwState = ThrowState.Returning;
        Destroy(_tetherSpringInstance);

        if(_tetheredEnemy == null) { return; }

        _tetheredEnemy.SetRagdollState(false);
        _tetheredEnemy = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(_harpoonRb.transform.position, Vector3.one * 2);
        Gizmos.DrawCube(_handleTransform.transform.position, Vector3.one * 2);
        Gizmos.DrawCube(_bendPoint.transform.position, Vector3.one * 2);
        Vector3 moveToForMiddleRope = new Vector3((_harpoonRb.transform.position.x + _handleTransform.position.x) / 2, (_harpoonRb.transform.position.y + _handleTransform.position.y) / 2, (_harpoonRb.transform.position.z + _handleTransform.position.z) / 2);
        Gizmos.DrawCube(moveToForMiddleRope, Vector3.one * 2);
    }

    #endregion

    #region Helper Classes

    public enum ThrowState
    {
        Attached,
        Throwing,
        Arrived,
        Stuck,
        GrabbingEnemy,
        Returning
    }

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