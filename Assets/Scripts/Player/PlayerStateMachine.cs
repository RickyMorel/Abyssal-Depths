using Rewired;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static Rewired.Controller;
using static Rewired.Platforms.Custom.CustomInputSource;

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
    private CapsuleCollider _capsuleCollider;
    private float _turnSmoothVelocity;
    private bool _isAttachedToShip;
    private float _fallSpeed;
    private float _timeSinceLastJump = float.MaxValue;

    #endregion

    #region Public Properties

    public bool IsAttachedToShip => _isAttachedToShip;
    public override bool IsShooting => _playerInput == null ? false : _playerInput.IsShooting;
    public float FallSpeed { get { return _fallSpeed; } set { _fallSpeed = value; } }
    public bool IsMoving;

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
        _capsuleCollider = GetComponent<CapsuleCollider>();
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

        _characterController.SimpleMove(Vector3.zero);
    }

    public override void FixedUpdate()
    {
        if (_playerInteraction.IsInteracting()) { return; }

        if (_canMove)
        {
            Move();
            ApplyGravity();
            RotateTowardsMove();
            AnimateMove();
        }

        //if (_characterController.velocity.y < Physics.gravity.y || _characterController.velocity.y > _jumpHeight) { _characterController.SimpleMove(Vector3.zero); }
    }

    public void OnDestroy()
    {
        if (_playerInput == null) { return; }

        _playerInput.OnJump -= HandleJump;
    }

    #endregion

    public override void Move()
    {
        IsMoving = false;
        Vector3 v3MoveInput = new Vector3(_playerInput.MoveDirection.x, 0f, _playerInput.MoveDirection.y);

        float cappedSpeed = _currentSpeed;
        float zMovement = CameraManager.Instance.IsInOrthoMode ? 0f : v3MoveInput.z * cappedSpeed;

        _moveDirection = new Vector3(v3MoveInput.x * cappedSpeed, 0f, zMovement);

        //Caps players movement on Z axis
        if (!WalkPlaneVisual.Instance.IsWithinBounds(transform.position + (_moveDirection * Time.deltaTime))) { _moveDirection.z = 0; }

        Vector3 blockedDir = Vector3.zero;

        if (Physics.Raycast(transform.position + (_capsuleCollider.height / 2f * Vector3.up), Vector3.right, out RaycastHit hitR, 1f, _collisionLayers)) { blockedDir = Vector3.right; }

        if (Physics.Raycast(transform.position + (_capsuleCollider.height / 2f * Vector3.up), Vector3.left, out RaycastHit hitL, 1f, _collisionLayers)) { blockedDir = Vector3.left; }

        if(blockedDir != Vector3.zero && v3MoveInput.x == blockedDir.x) { Debug.Log("Blocked: " + blockedDir); return; }

        if (Mathf.Abs(_characterController.velocity.x) > 1f) { return; }
        //if(_characterController.velocity.y < -0.3f && _fallSpeed < -9f) { return; }

        _characterController.Move(_moveDirection * Time.deltaTime);
        IsMoving = true;
        Debug.Log("X Move: " + blockedDir);
    }

    private void ApplyGravity()
    {
        Vector3 gravity = new Vector3(0f, _gravityIntensity, 0f) * Time.deltaTime;

        _fallSpeed += gravity.y;

        if (_fallSpeed < Physics.gravity.y) { _fallSpeed = Physics.gravity.y; }

        //Stops jumping when hitting something from above
        if (Physics.Raycast(transform.position + (_capsuleCollider.height * Vector3.up),
            transform.up, out RaycastHit hitU, 0.5f, _collisionLayers) && _fallSpeed > 0f) { _fallSpeed = 0f; return; }

        //if (_characterController.velocity.y < Physics.gravity.y || _characterController.velocity.y > _jumpHeight) { _characterController.SimpleMove(Vector3.zero); return; }

        _characterController.Move(new Vector3(0f, _fallSpeed, 0f) * Time.deltaTime);
        //Debug.Log($"Velocity: ${_characterController.velocity} ; FallSpeed: {FallSpeed}");
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
