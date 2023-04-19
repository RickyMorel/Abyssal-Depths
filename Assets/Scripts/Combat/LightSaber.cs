using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSaber : MeleeWeapon
{
    #region Editor Fields

    [Header("Transforms")]
    [SerializeField] private Transform _originalPosition;
    [SerializeField] private Transform _rotationPoint;
    [SerializeField] private Transform _moveToForLightSaber;
    [Header("Object related")]
    [SerializeField] private GameObject _handle;
    [SerializeField] private GameObject _lightSaber;
    [SerializeField] private BoxCollider _bladeBoxCollider;
    [Header("Floats")]
    [SerializeField] private float _flyingSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _returnLightSaberAfterSeconds;


    #endregion

    #region Private Variables

    private Transform _shootToPosition;
    private bool _isBladeOut = false;
    private bool _boomerangThrow = false;
    private bool _canShootSaber = true;

    #endregion

    #region Unity Loops

    public override void Start()
    {
        base.Start();

        _bladeBoxCollider.enabled = false;

        var timelineAsset = playableDirector.playableDirector as TimelineAsset;
        var track = timelineAsset.GetOutputTracks().FirstOrDefault(t => t.name == "MyTrackName");

        playableDirector.SetGenericBinding(track, myAnimator);

    }

    public override void FixedUpdate()
    {
        if (_weapon.CurrentPlayer != null && !_isBladeOut) 
        {
            _isBladeOut = true;
            TimelinesManager.Instance.LightSaberBlade.gameObject.transform.SetParent(_handle.transform);
            TimelinesManager.Instance.LightSaberBlade.gameObject.transform.localPosition = new Vector3(0, 0, 0);
            TimelinesManager.Instance.LightSaberOut.Play();
            _bladeBoxCollider.enabled = true;
        }
        else if (_weapon.CurrentPlayer == null && _isBladeOut) 
        {
            _isBladeOut = false;
            TimelinesManager.Instance.LightSaberBlade.gameObject.transform.localPosition = new Vector3(0, 0, 0);
            TimelinesManager.Instance.LightSaberIn.Play();
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
        _shootToPosition = _moveToForLightSaber;
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
            _handle.transform.SetParent(_lightSaber.transform);
            _lightSaber.transform.position = Vector3.MoveTowards(_lightSaber.transform.position, _shootToPosition.position, Time.deltaTime * _flyingSpeed);
            if (!_canShootSaber) { _lightSaber.transform.Rotate(Vector3.right * (_rotationSpeed * Time.deltaTime)); }
        }
        if (!_boomerangThrow) 
        { 
            _lightSaber.transform.position = Vector3.MoveTowards(_lightSaber.transform.position, _originalPosition.position, Time.deltaTime * _flyingSpeed);
            if (!_canShootSaber) { _lightSaber.transform.Rotate(Vector3.right * (_rotationSpeed * Time.deltaTime)); }
        }
        if (_handle.transform.position == _originalPosition.position && !_boomerangThrow) 
        {
            _lightSaber.transform.SetParent(_handle.transform);
            _handle.transform.SetParent(_rotationPoint.transform);
            _handle.transform.localRotation = Quaternion.identity;
            _canShootSaber = true;
            _weapon.ShouldRotate = true;
        }
    }

    private IEnumerator BoomerangReturn()
    {
        _boomerangThrow = true;
        _handle.transform.SetParent(null);
        yield return new WaitForSeconds(_returnLightSaberAfterSeconds);
        _boomerangThrow = false;
    }
}