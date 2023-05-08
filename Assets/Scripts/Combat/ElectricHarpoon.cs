using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ElectricHarpoon : MeleeWeapon
{
    #region Editor Fields

    [Header("Transforms")]
    [SerializeField] private Transform _moveToForLightSaber;
    [SerializeField] private Transform _handleTransform;
    [SerializeField] private Rigidbody _harpoonRb;
    [Header("Floats")]
    [SerializeField] private float _flyingSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _returnLightSaberAfterSeconds;
    [Header("Animation")]
    [SerializeField] private PlayableDirector _lightSaberOut;
    [SerializeField] private PlayableDirector _lightSaberIn;
    [Header("GameObject Related")]
    [SerializeField] private Collider _trackEnemiesZone;

    #endregion

    #region Private Variables

    private List<Transform> _enemyTransforms = new List<Transform>();
    private Vector3 _originalHarpoonPosition;
    private Quaternion _originalHarpoonRotation;
    [SerializeField] private ThrowState _throwState = ThrowState.Attached;

    #endregion

    #region Public Properties


    #endregion

    #region Unity Loops

    public override void Start()
    {
        base.Start();

        _rotationSpeed = _rotationSpeed * _rotationSpeed;
        _originalHarpoonPosition = _harpoonRb.transform.localPosition;
        _originalHarpoonRotation = _harpoonRb.transform.localRotation;
    }

    public override void Update()
    {
        base.Update();

        _harpoonRb.isKinematic = _throwState == ThrowState.Attached;

        ThrowLightSaber();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out AIHealth enemy)) { return; }

        if (_enemyTransforms.Contains(enemy.gameObject.transform)) { return; }

        _enemyTransforms.Add(enemy.gameObject.transform);
    }

    #endregion

    public override void Shoot()
    {
        if (_throwState != ThrowState.Attached) 
        { 
            if(_throwState == ThrowState.Arrived) { ReturnHarpoon(); }

            return; 
        }

        _throwState = ThrowState.Throwing;
        _weapon.ShouldRotate = false;
        StartCoroutine(CheckForEnemyTransforms());
    }

    public override void CheckShootInput()
    {
        if (_weapon.CurrentPlayer.IsUsing)
        {
            Shoot();
        }
    }
    
    private void ThrowLightSaber()
    {
        Transform moveToCurrentPosition = _moveToForLightSaber;

        if (_throwState == ThrowState.Throwing && _harpoonRb.transform.position != moveToCurrentPosition.position)
        {
            _harpoonRb.transform.SetParent(null);
            Vector3 forceDir = moveToCurrentPosition.position - _harpoonRb.transform.position;
            _harpoonRb.AddForce(forceDir.normalized * _flyingSpeed * Time.deltaTime);
        }
        if (_throwState == ThrowState.Throwing && Vector3.Distance(_harpoonRb.transform.position, moveToCurrentPosition.position) < 2f)
        {
            _throwState = ThrowState.Arrived;
        }

        LightSaberReturnToWeapon();
    }

    private void LightSaberReturnToWeapon()
    {
        if (_harpoonRb.transform.position != _handleTransform.position && _throwState == ThrowState.Returning)
        {
            _harpoonRb.transform.position = Vector3.MoveTowards(_harpoonRb.transform.position, _handleTransform.position, Time.deltaTime * _flyingSpeed);
        }
        if (_harpoonRb.transform.position == _handleTransform.position && _throwState == ThrowState.Returning)
        {
            _harpoonRb.transform.SetParent(_handleTransform);
            _harpoonRb.transform.localPosition = _originalHarpoonPosition;
            _harpoonRb.transform.localRotation = _originalHarpoonRotation;
            _weapon.ShouldRotate = true;
            _throwState = ThrowState.Attached;
        }
    }

    private void ReturnHarpoon()
    {
        _throwState = ThrowState.Returning;
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
        Returning
    }
}