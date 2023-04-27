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
    [SerializeField] private Transform _lightSaberVisual;
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

    int _enemyIndex = 0;

    private bool _isBladeOut = false;
    private bool _boomerangThrow = false;
    private bool _canShootSaber = true;
    private bool _checksCurrentPlayer = true;

    private Vector3 _handleOriginalLocalPosition;

    private List<Transform> _enemiesTransform = new List<Transform>();

    #endregion

    #region Unity Loops

    public override void Start()
    {
        base.Start();

        _rotationSpeed = _rotationSpeed * _rotationSpeed;
        _handleOriginalLocalPosition = _handleTransform.localPosition;
    }

    public override void FixedUpdate()
    {
        if (_weapon.CurrentPlayer != null && !_isBladeOut && _checksCurrentPlayer) 
        {
            Debug.Log("Wha da hell");
            _isBladeOut = true;
            _lightSaberOut.Play();
        }
        else if (_weapon.CurrentPlayer == null && _isBladeOut) 
        {
            _isBladeOut = false;
            _lightSaberIn.Play();
        }
    }

    public override void Update()
    {
        base.Update();
        ThrowLightSaber();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out AIHealth enemy)) { return; }

        _enemiesTransform.Add(enemy.gameObject.transform);
    }

    #endregion

    public override void Shoot()
    {
        if (!_canShootSaber) { return; }

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
        if (_enemiesTransform.Count == 0) 
        {
            Transform moveToCurrentPosition = _moveToForLightSaber;

            if (_boomerangThrow && _lightSaberVisual.position != moveToCurrentPosition.position)
            {
                _lightSaberVisual.SetParent(null);
                _lightSaberVisual.position = Vector3.MoveTowards(_lightSaberVisual.position, moveToCurrentPosition.position, Time.deltaTime * _flyingSpeed);
                _lightSaberVisual.Rotate(Vector3.right * (_rotationSpeed * Time.deltaTime));
            }
            if (_boomerangThrow && _lightSaberVisual.position == moveToCurrentPosition.position)
            {
                if (!_canShootSaber) { _lightSaberVisual.Rotate(Vector3.right * (_rotationSpeed * Time.deltaTime)); }
                StartCoroutine(BoomerangReturn());
            }

            LightSaberReturnToWeapon();
        }
        else
        {
            if (_boomerangThrow && _enemyIndex != _enemiesTransform.Count)
            {
                _lightSaberVisual.SetParent(null);
                _lightSaberVisual.position = Vector3.MoveTowards(_lightSaberVisual.position, _enemiesTransform[_enemyIndex].position, Time.deltaTime * _flyingSpeed);
                _lightSaberVisual.Rotate(Vector3.right * (_rotationSpeed * Time.deltaTime));
                if (_lightSaberVisual.position == _enemiesTransform[_enemyIndex].position) { _enemyIndex++; }
            }
            if (_boomerangThrow && _enemyIndex == _enemiesTransform.Count)
            {
                if (!_canShootSaber) { _lightSaberVisual.Rotate(Vector3.right * (_rotationSpeed * Time.deltaTime)); }
                StartCoroutine(BoomerangReturn());
            }

            LightSaberReturnToWeapon();
        }
    }

    private void LightSaberReturnToWeapon()
    {
        if (_lightSaberVisual.position != _centerPosition.position && !_boomerangThrow)
        {
            _lightSaberVisual.SetParent(_handleTransform);
            _lightSaberVisual.position = Vector3.MoveTowards(_lightSaberVisual.position, _centerPosition.position, Time.deltaTime * _flyingSpeed);
            _handleTransform.localPosition = Vector3.MoveTowards(_handleTransform.localPosition, Vector3.zero, Time.deltaTime * _flyingSpeed);
            _handleTransform.Rotate(Vector3.right * (_rotationSpeed * Time.deltaTime));
        }
        if (_lightSaberVisual.position == _centerPosition.position && !_boomerangThrow)
        {
            _handleTransform.SetParent(_lightSaberVisual);
            _lightSaberVisual.SetParent(_rotationPoint.transform);
            _handleTransform.localPosition = _handleOriginalLocalPosition;
            _lightSaberVisual.localRotation = Quaternion.Euler(0, 0, 0);
            _handleTransform.localRotation = Quaternion.Euler(0, 0, 0);
            _canShootSaber = true;
            _weapon.ShouldRotate = true;
            _checksCurrentPlayer = true;
        }
    }

    private IEnumerator BoomerangReturn()
    {
        yield return new WaitForSeconds(_returnLightSaberAfterSeconds);
        _boomerangThrow = false;
        _checksCurrentPlayer = false;
        _isBladeOut = false;
        _lightSaberIn.Play();
    }

    private IEnumerator CheckForEnemyTransforms()
    {
        _enemyIndex = 0;
        _trackEnemiesZone.enabled = true;
        yield return new WaitForSeconds(0.1f);
        _trackEnemiesZone.enabled = false;
        _boomerangThrow = true;
        _lightSaberVisual.transform.SetParent(null);
        _canShootSaber = false;
        _weapon.ShouldRotate = false;
    }
}