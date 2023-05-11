using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class ElectricHarpoon : MeleeWeapon
{
    #region Editor Fields

    [Header("Transforms")]
    [SerializeField] private Transform _moveToForLightSaber;
    [SerializeField] private Transform _handleTransform;
    [SerializeField] private Rigidbody _harpoonRb;
    [Header("Floats")]
    [SerializeField] private float _flyingSpeed;
    [SerializeField] private float _grappleSpeed;
    [SerializeField] private float _maxTetherDistance;
    [Header("GameObject Related")]
    [SerializeField] private Collider _trackEnemiesZone;
    [SerializeField] private GameObject _electructionZonePrefab;

    #endregion

    #region Private Variables

    private List<Transform> _enemyTransforms = new List<Transform>();
    private Vector3 _originalHarpoonPosition;
    private Quaternion _originalHarpoonRotation;
    [SerializeField] private ThrowState _throwState = ThrowState.Attached;
    private bool _prevInputState;
    private float _timePassedReturning;
    private LineRenderer _lr;
    private SpringJoint _tetherSpringInstance;
    private AIStateMachine _tetheredEnemy;
    private WeaponAttackHitBox _electrocutionZoneInstance;

    #endregion

    #region Public Properties


    #endregion

    #region Unity Loops

    public override void Start()
    {
        base.Start();

        _lr = GetComponent<LineRenderer>();
        _lr.positionCount = 2;
        _electrocutionZoneInstance = Instantiate(_electructionZonePrefab).GetComponent<WeaponAttackHitBox>();
        _electrocutionZoneInstance.Initialize(_weapon, _weapon.InteractableHealth, this);

        _originalHarpoonPosition = _harpoonRb.transform.localPosition;
        _originalHarpoonRotation = _harpoonRb.transform.localRotation;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        _harpoonRb.isKinematic = _throwState == ThrowState.Attached || _throwState == ThrowState.Returning || _throwState == ThrowState.Stuck || _throwState == ThrowState.GrabbingEnemy;

        if(_throwState == ThrowState.GrabbingEnemy && _tetheredEnemy != null) { _harpoonRb.transform.position = _tetheredEnemy.transform.position; }

        ThrowLightSaber();
        if (_throwState == ThrowState.Stuck || _throwState == ThrowState.GrabbingEnemy) { AdjustElectrocutionZone(); }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out AIHealth enemy)) { return; }

        if (_enemyTransforms.Contains(enemy.gameObject.transform)) { return; }

        _enemyTransforms.Add(enemy.gameObject.transform);
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

    public override void CheckShootInput()
    {
        if (_weapon.CurrentPlayer.IsUsing_2 && _throwState == ThrowState.Stuck)
        {
            Vector3 grappleDirection = _harpoonRb.transform.position - Ship.Instance.transform.position;
            Vector3 finalForce = grappleDirection.normalized * _grappleSpeed * Time.deltaTime;
            Ship.Instance.AddForceToShip(finalForce, ForceMode.Force);
            return;
        }

        if (_weapon.CurrentPlayer.IsUsing == _prevInputState) { return; }

        _prevInputState = _weapon.CurrentPlayer.IsUsing;

        if (_weapon.CurrentPlayer.IsUsing)
        {
            Shoot();
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

        StartCoroutine(CheckForEnemyTransforms());
    }
    
    private void ThrowLightSaber()
    {
        if (_throwState == ThrowState.Arrived) { return; }

        Transform moveToCurrentPosition = _moveToForLightSaber;

        if (_throwState == ThrowState.Throwing)
        {
            Vector3 forceDir = _moveToForLightSaber.position - _harpoonRb.transform.position;
            _harpoonRb.AddForce(forceDir.normalized * _flyingSpeed, ForceMode.Force);
        }

        if (_throwState == ThrowState.Throwing && Vector3.Distance(_harpoonRb.transform.position, moveToCurrentPosition.position) < 2f)
        {
            _throwState = ThrowState.Arrived;
        }

        LightSaberReturnToWeapon();
    }

    private void AdjustElectrocutionZone()
    {
        _electrocutionZoneInstance.transform.localScale =
            new Vector3(
                Vector3.Distance(_harpoonRb.transform.position, _handleTransform.position),
                _electrocutionZoneInstance.transform.localScale.y,
                _electrocutionZoneInstance.transform.localScale.z
                );

        _electrocutionZoneInstance.transform.position =
            new Vector3(
                (_harpoonRb.transform.position.x + _handleTransform.transform.position.x) / 2f,
                _handleTransform.transform.position.y,
                _handleTransform.transform.position.z
                );

        Vector3 direction = _harpoonRb.transform.position - _handleTransform.position;
        float angle = Vector3.Angle(direction, transform.up);

        //Determine if angle is negative
        Vector3 cross = Vector3.Cross(direction, transform.up);
        if (cross.y < 0)
        {
            angle = -angle;
        }

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

        _electrocutionZoneInstance.transform.rotation = targetRotation;
    }

    private void LightSaberReturnToWeapon()
    {
        if (_harpoonRb.transform.position != _handleTransform.position && _throwState == ThrowState.Returning)
        {
            _timePassedReturning += Time.deltaTime;
            _harpoonRb.transform.position = Vector3.MoveTowards(_harpoonRb.transform.position, _handleTransform.position, Time.deltaTime * (_flyingSpeed / 10f) * _timePassedReturning);
            //_harpoonRb.transform.rotation = Quaternion.AngleAxis(Vector3.Angle(_harpoonRb.transform.position, _handleTransform.position)
            //    * Time.deltaTime * (_flyingSpeed/10f), Vector3.forward);
        }
        if (_harpoonRb.transform.position == _handleTransform.position && _throwState == ThrowState.Returning)
        {
            _harpoonRb.transform.SetParent(_handleTransform);
            _harpoonRb.transform.localPosition = _originalHarpoonPosition;
            _harpoonRb.transform.localRotation = _originalHarpoonRotation;
            _weapon.ShouldRotate = true;
            _throwState = ThrowState.Attached;
            _timePassedReturning = 0f;
        }
    }

    private void DrawRope()
    {
        _lr.SetPosition(0, _harpoonRb.transform.position);
        _lr.SetPosition(1, _handleTransform.position);
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

    private IEnumerator CheckForEnemyTransforms()
    {
        _enemyTransforms.Clear();
        _trackEnemiesZone.enabled = true;
        yield return new WaitForSeconds(Time.deltaTime*100);
        _trackEnemiesZone.enabled = false;    
    }

    public enum ThrowState
    {
        Attached,
        Throwing,
        Arrived,
        Stuck,
        GrabbingEnemy,
        Returning
    }
}