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

    private int _enemyIndex = 0;

    private bool _isBladeOut = false;
    private bool _boomerangThrow = false;
    private bool _isBoomerangReturning = false;
    private bool _canShootSaber = true;
    private bool _checksCurrentPlayer = true;

    private Vector3 _handleOriginalLocalPosition;

    private List<Transform> _enemyTransforms = new List<Transform>();

    #endregion

    #region Public Properties

    public bool IsBladeOut => _isBladeOut;
    public bool BoomerangThrow => _boomerangThrow;

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

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out AIHealth enemy)) { return; }

        if (_enemyTransforms.Contains(enemy.gameObject.transform)) { return; }

        _enemyTransforms.Add(enemy.gameObject.transform);
    }

    #endregion

    public override void Shoot()
    {
        if (!_canShootSaber) { return; }
        if (_boomerangThrow) { return; }
        if (_isBoomerangReturning) { return; }

        _canShootSaber = false;
        _boomerangThrow = true;
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
        //If there is no enemy, travel a predetermined position
        if (_enemyTransforms.Count == 0) 
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
                _lightSaberVisual.Rotate(Vector3.right * (_rotationSpeed * Time.deltaTime));
                StartCoroutine(BoomerangReturn());
            }

            LightSaberReturnToWeapon();
        }
        else
        {
            if (_boomerangThrow && _enemyIndex != _enemyTransforms.Count)
            {
                _lightSaberVisual.SetParent(null);
                _lightSaberVisual.position = Vector3.MoveTowards(_lightSaberVisual.position, _enemyTransforms[_enemyIndex].position, Time.deltaTime * _flyingSpeed);
                _lightSaberVisual.Rotate(Vector3.right * (_rotationSpeed * Time.deltaTime));
                if (_lightSaberVisual.position == _enemyTransforms[_enemyIndex].position) { _enemyIndex++; }
            }
            if (_boomerangThrow && _enemyIndex == _enemyTransforms.Count)
            {
                _lightSaberVisual.Rotate(Vector3.right * (_rotationSpeed * Time.deltaTime));
                StartCoroutine(BoomerangReturn());
            }

            LightSaberReturnToWeapon();
        }
    }

    private void LightSaberReturnToWeapon()
    {
        if (_lightSaberVisual.position != _centerPosition.position && _isBoomerangReturning)
        {
            _lightSaberVisual.SetParent(_handleTransform);
            _lightSaberVisual.position = Vector3.MoveTowards(_lightSaberVisual.position, _centerPosition.position, Time.deltaTime * _flyingSpeed);
            _handleTransform.localPosition = Vector3.MoveTowards(_handleTransform.localPosition, Vector3.zero, Time.deltaTime * _flyingSpeed);
            _handleTransform.Rotate(Vector3.right * (_rotationSpeed * Time.deltaTime));
        }
        if (_lightSaberVisual.position == _centerPosition.position && _isBoomerangReturning)
        {
            _handleTransform.SetParent(_lightSaberVisual);
            _lightSaberVisual.SetParent(_rotationPoint.transform);
            _handleTransform.localPosition = _handleOriginalLocalPosition;
            _lightSaberVisual.localRotation = Quaternion.Euler(0, 0, 0);
            _handleTransform.localRotation = Quaternion.Euler(0, 0, 0);
            _weapon.ShouldRotate = true;
            _checksCurrentPlayer = true;
            _isBoomerangReturning = false;
            StartCoroutine(CanShootSaberDelayCoroutine());
        }
    }

    private IEnumerator CanShootSaberDelayCoroutine()
    {
        yield return new WaitForSeconds(1f);

        _canShootSaber = true;
    }

    //This is for the lightsaber going in animation, and for the boomerangthrow boolean, that way, in the function throwlightsaber, it knows it has to go back.
    private IEnumerator BoomerangReturn()
    {
        yield return new WaitForSeconds(_returnLightSaberAfterSeconds);
        _boomerangThrow = false;
        _isBoomerangReturning = true;
        _checksCurrentPlayer = false;
        if (_isBladeOut)
        {
            _isBladeOut = false;
            _lightSaberIn.Play();
        }
    }

    private IEnumerator CheckForEnemyTransforms()
    {
        _enemyTransforms.Clear();
        _enemyIndex = 0;
        _trackEnemiesZone.enabled = true;
        yield return new WaitForSeconds(Time.deltaTime*100);
        _trackEnemiesZone.enabled = false;    }
}