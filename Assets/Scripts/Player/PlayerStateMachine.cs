using Rewired;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static Rewired.Controller;

[RequireComponent(typeof(PlayerCarryController))]
[RequireComponent(typeof(MovingObjectAttacher))]
[RequireComponent(typeof(CharacterController))]
public class PlayerStateMachine : BaseStateMachine
{
    #region Editor Fields

    [SerializeField] private LayerMask _collisionLayers;
    [SerializeField] private float _timeBetweenJumps = 1f;

    #endregion

    #region Private Variables

    private PlayerInputHandler _playerInput;
    private CharacterController _characterController;
    private MovingObjectAttacher _platformAttacher;
    private float _turnSmoothVelocity;
    private bool _isAttachedToShip;
    private float _fallSpeed;
    private float _timeSinceLastJump = float.MaxValue;

    #endregion

    #region Public Properties

    public bool IsAttachedToShip => _isAttachedToShip;
    public override bool IsShooting => _playerInput == null ? false : _playerInput.IsShooting;
    public float FallSpeed { get { return _fallSpeed; } set { _fallSpeed = value; } }

    #endregion

    #region Unity Loops

    public override void Awake()
    {
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
        _damageable = GetComponent<Damageable>();
    }

    public override void Start()
    {
        base.Start();

        _playerInput = GetComponent<PlayerInputHandler>();
        _playerInteraction = GetComponent<PlayerInteractionController>();
        _playerCarryController = GetComponent<PlayerCarryController>();
        _characterController = GetComponent<CharacterController>();
        _platformAttacher = GetComponent<MovingObjectAttacher>();
        _characterController.detectCollisions = false;

        _playerInput.OnJump += HandleJump;
    }

    public void AttachToShip(bool isAttached)
    {
        _isAttachedToShip = isAttached;

        if (isAttached)
        {
            transform.SetParent(Ship.Instance.transform);
            _platformAttacher.SetActivePlatform(Ship.Instance.transform);
        }
        else
        {
            transform.parent = null;
            _platformAttacher.SetActivePlatform(null);
        }
    }

    public override void Update()
    {
        _timeSinceLastJump += Time.deltaTime;

        _currentState.UpdateStates();
    }

    public override void FixedUpdate()
    {
        if (_playerInteraction.IsInteracting()) { return; }

        if (_canMove)
        {
            Move();
            RotateTowardsMove();
            AnimateMove();
        }
    }

    public void OnDestroy()
    {
        if (_playerInput == null) { return; }

        _playerInput.OnJump -= HandleJump;
    }

    #endregion

    public override void Move()
    {
        Vector3 v3MoveInput = new Vector3(_playerInput.MoveDirection.x, 0f, _playerInput.MoveDirection.y);

        //if(_currentBlockedDirection != Vector3.zero && v3MoveInput == _currentBlockedDirection) { return; }

        float cappedSpeed = _currentSpeed;
        float zMovement = CameraManager.Instance.IsInOrthoMode ? 0f : v3MoveInput.z * cappedSpeed;
        Vector3 gravity = new Vector3(0f, _gravityIntensity, 0f) * Time.deltaTime;
        _fallSpeed += gravity.y;
        if(_fallSpeed < Physics.gravity.y) { _fallSpeed = Physics.gravity.y; }

        _moveDirection = new Vector3(v3MoveInput.x * cappedSpeed, 0f, zMovement);

        //Caps players movement on Z axis
        if (!WalkPlaneVisual.Instance.IsWithinBounds(transform.position + (_moveDirection * Time.deltaTime))) { _moveDirection.z = 0; }

        _moveDirection.y = _fallSpeed;

        //transform.position += _moveDirection;
        _characterController.Move(_moveDirection * Time.deltaTime);
    }

    public override void RotateTowardsMove()
    {
        float targetAngle = Mathf.Atan2(_playerInput.MoveDirection.x, _playerInput.MoveDirection.y) * Mathf.Rad2Deg;

        if (_playerInput.MoveDirection.magnitude == 0)
            targetAngle = 180f;

        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    public override void AnimateMove()
    {
        _anim.SetFloat("Movement", _playerInput.MoveDirection.magnitude);
    }

    private void HandleJump()
    {
        if (_isJumpPressed) { return; }

        if(_timeSinceLastJump < _timeBetweenJumps) { return; }

        _timeSinceLastJump = 0f;

        StartCoroutine(SetJumpCoroutine());
    }

    private IEnumerator SetJumpCoroutine()
    {
        _isJumpPressed = true;

        yield return new WaitForSeconds(0.2f);

        _isJumpPressed = false;
    }
}
