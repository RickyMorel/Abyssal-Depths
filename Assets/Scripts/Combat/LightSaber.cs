using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class LightSaber : MeleeWeapon
{
    #region Editor Fields

    [Header("Transforms")]
    [SerializeField] private Transform _centerPosition;
    [SerializeField] private Transform _rotationPoint;
    [SerializeField] private Transform _moveToForLightSaber;
    [SerializeField] private Transform _handleTransform;
    [Header("Object related")]
    [SerializeField] private GameObject _lightSaberVisual;
    [SerializeField] private BoxCollider _bladeBoxCollider;
    [Header("Floats")]
    [SerializeField] private float _flyingSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _returnLightSaberAfterSeconds;
    [Header("Animation")]
    [SerializeField] private PlayableDirector _lightSaberOut;
    [SerializeField] private PlayableDirector _lightSaberIn;

    #endregion

    #region Private Variables

    private bool _isBladeOut = false;
    private bool _boomerangThrow = false;
    private bool _canShootSaber = true;
    private bool _checksCurrentPlayer = true;

    private Transform _handleOriginalLocalPosition;

    #endregion

    #region Unity Loops

    public override void Start()
    {
        base.Start();

        _handleOriginalLocalPosition.position = _handleTransform.localPosition;
        _bladeBoxCollider.enabled = false;
    }

    public override void FixedUpdate()
    {
        if (_weapon.CurrentPlayer != null && !_isBladeOut && _checksCurrentPlayer) 
        {
            _isBladeOut = true;
            _lightSaberOut.Play();
            _bladeBoxCollider.enabled = true;
        }
        else if (_weapon.CurrentPlayer == null && _isBladeOut) 
        {
            _isBladeOut = false;
            _lightSaberIn.Play();
            _bladeBoxCollider.enabled = false;
        }
    }

    public override void Update()
    {
        base.Update();

        ThrowLightSaber();
    }

    #endregion

    public override void Shoot()
    {
        if (!_canShootSaber) { return; }

        _canShootSaber = false;
        _weapon.ShouldRotate = false;
        StartCoroutine(BoomerangReturn());
    }

    public override void CheckShootInput()
    {
        if (_weapon.CurrentPlayer.IsUsing)
        {
            Shoot();
        }
    }
    //To do: make the rotation right, lightsaber must not collide with ship, make it slower, have a list of enemies that it has to go first before returning, use slerp so that it doesn't go in a straight line, it must close when returning.
    private void ThrowLightSaber()
    {
        if (_boomerangThrow) 
        {
            _lightSaberVisual.transform.SetParent(null);
            _lightSaberVisual.transform.position = Vector3.MoveTowards(_lightSaberVisual.transform.position, _moveToForLightSaber.position, Time.deltaTime * _flyingSpeed);
            if (!_canShootSaber) { _lightSaberVisual.transform.Rotate(Vector3.right * (_rotationSpeed * Time.deltaTime)); }
        }
        if (_lightSaberVisual.transform.position != _centerPosition.position && !_boomerangThrow) 
        { 
            _lightSaberVisual.transform.position = Vector3.MoveTowards(_lightSaberVisual.transform.position, _centerPosition.position, Time.deltaTime * _flyingSpeed);
            _handleTransform.localPosition = Vector3.MoveTowards(_handleTransform.localPosition, Vector3.zero, Time.deltaTime * _flyingSpeed);
            if (!_canShootSaber) { _lightSaberVisual.transform.Rotate(Vector3.right * (_rotationSpeed * Time.deltaTime)); }
        }
        if (_lightSaberVisual.transform.position == _centerPosition.position && !_boomerangThrow)
        {
            _handleTransform.localPosition = _handleOriginalLocalPosition.position;
            _lightSaberVisual.transform.SetParent(_rotationPoint.transform);
            _lightSaberVisual.transform.localRotation = Quaternion.Euler(0, 0, 0);
            _canShootSaber = true;
            _weapon.ShouldRotate = true;
            _checksCurrentPlayer = true;
        }
    }

    private IEnumerator BoomerangReturn()
    {
        _boomerangThrow = true;
        _lightSaberVisual.transform.SetParent(null);
        yield return new WaitForSeconds(_returnLightSaberAfterSeconds);
        _boomerangThrow = false;
        _checksCurrentPlayer = false;
        _isBladeOut = false;
        _lightSaberIn.Play();
        _bladeBoxCollider.enabled = false;
    }
}