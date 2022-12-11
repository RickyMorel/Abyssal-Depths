using Rewired;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static Rewired.Controller;

[RequireComponent(typeof(PlayerCarryController))]
public class PlayerStateMachine : BaseStateMachine
{
    #region Editor Fields

    [SerializeField] private LayerMask _collisionLayers;
    [SerializeField] private bool _isOrthoMode = true; 

    #endregion

    #region Private Variables

    private PlayerInputHandler _playerInput;
    private CapsuleCollider _capsuleCollider;
    private float _turnSmoothVelocity;
    private bool _isAttachedToShip;
    private Vector3 _fallVelocity;
    public bool _applyGravity;
    private Vector3 _currentBlockedDirection;

    #endregion

    #region Public Properties

    public bool IsAttachedToShip => _isAttachedToShip;
    public override bool IsShooting => _playerInput == null ? false : _playerInput.IsShooting;
    public Vector3 FallVelocity { get { return _fallVelocity; } set { _fallVelocity = value; } }

    #endregion

    #region Unity Loops

    public override void Awake()
    {
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
    }

    public override void Start()
    {
        _playerInput = GetComponent<PlayerInputHandler>();
        _playerInteraction = GetComponent<PlayerInteractionController>();
        _playerRagdoll = GetComponent<PlayerRagdoll>();
        _playerHealth = GetComponent<PlayerHealth>();
        _playerCarryController = GetComponent<PlayerCarryController>();
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        //AttachToShip(true);

        _playerInput.OnJump += HandleJump;
    }

    public void AttachToShip(bool isAttached)
    {
        _isAttachedToShip = isAttached;

        if (isAttached)
        {
            transform.SetParent(Ship.Instance.transform);
        }
        else
        {
            transform.parent = null;
        }
    }

    public override void Update()
    {
        _currentState.UpdateStates();
    }

    public override void FixedUpdate()
    {
        if (_playerInteraction.IsInteracting()) { return; }

        if (_canMove)
        {
            CustomCollisionDecection();
            Move();
            RotateTowardsMove();
            AnimateMove();
            ApplyGravity();
        }

        CheckIfFellOutOfShip();
    }

    public void OnDestroy()
    {
        if (_playerInput == null) { return; }

        _playerInput.OnJump -= HandleJump;
    }

    #endregion

    private void CustomCollisionDecection()
    {
        float height = _capsuleCollider.height;
        float width = 0.5f;
        float moveVerticalAmount = 2f * Time.deltaTime;
        float feetClippingHeightRatio = 4f;
        Vector3 RaycastDrawPosition = transform.position + _capsuleCollider.height / 2f * Vector3.up;
        _currentBlockedDirection = Vector3.zero;

        if (Physics.Raycast(RaycastDrawPosition, Vector3.right, out RaycastHit hitR, width, _collisionLayers))
        {
            _currentBlockedDirection = Vector3.right;
        }

        if (Physics.Raycast(RaycastDrawPosition, Vector3.left, out RaycastHit hitL, width, _collisionLayers))
        {
            _currentBlockedDirection = Vector3.left;
        }

        if (Physics.Raycast(RaycastDrawPosition, Vector3.up, out RaycastHit hit2U, height/2f, _collisionLayers))
        {
            _fallVelocity = Vector3.zero;
        }

        if (Physics.Raycast(transform.position + _capsuleCollider.height/feetClippingHeightRatio * Vector3.up, Vector3.down, out RaycastHit hit2D, height/feetClippingHeightRatio, _collisionLayers))
        {
            if (_fallVelocity != Vector3.zero) { transform.position -= new Vector3(0f, -moveVerticalAmount, 0f); }
            _applyGravity = false;
        }
        else
        {
            _applyGravity = true;
        }

        Debug.DrawRay(RaycastDrawPosition, Vector3.up * height / 2f, Color.red);
        Debug.DrawRay(transform.position + _capsuleCollider.height/feetClippingHeightRatio * Vector3.up, Vector3.down * height/feetClippingHeightRatio, Color.cyan);
    }

    public override void Move()
    {
        Vector3 v3MoveInput = new Vector3(_playerInput.MoveDirection.x, 0f, _playerInput.MoveDirection.y);

        if(_currentBlockedDirection != Vector3.zero && v3MoveInput == _currentBlockedDirection) { return; }

        float cappedSpeed = _currentSpeed / 20;
        float zMovement = _isOrthoMode ? 0f : _playerInput.MoveDirection.y * cappedSpeed;
        _moveDirection = new Vector3(_playerInput.MoveDirection.x * cappedSpeed, 0f, zMovement);
        transform.position += _moveDirection;
    }

    public void ApplyGravity()
    {
        if (!_applyGravity && !_isJumpPressed && _isGrounded) { _fallVelocity = Vector3.zero; return; }
        else
        {
            _fallVelocity += Physics.gravity * Time.deltaTime;
            transform.position += _fallVelocity * Time.deltaTime;
        }
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

    private void CheckIfFellOutOfShip()
    {
        if (!_isAttachedToShip) { return; }

        if(Vector3.Distance(transform.position, Ship.Instance.transform.position) < 50) { return; }

        transform.localPosition = Vector3.zero;
    }

    private void HandleJump()
    {
        if (_isJumpPressed) { return; }

        StartCoroutine(SetJumpCoroutine());
    }

    private IEnumerator SetJumpCoroutine()
    {
        _isJumpPressed = true;

        yield return new WaitForSeconds(0.2f);

        _isJumpPressed = false;
    }
}
