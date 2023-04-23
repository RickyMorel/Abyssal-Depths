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
    [SerializeField] private float _timeBetweenJumps = 1f;

    #endregion

    #region Private Variables

    private PlayerInputHandler _playerInput;
    private CapsuleCollider _capsuleCollider;
    private float _turnSmoothVelocity;
    private bool _isAttachedToShip;
    private Vector3 _fallVelocity;
    private bool _applyGravity;
    private Vector3 _currentBlockedDirection;
    private float _timeSinceLastJump = float.MaxValue;

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
        _damageable = GetComponent<Damageable>();
    }

    public override void Start()
    {
        base.Start();

        _playerInput = GetComponent<PlayerInputHandler>();
        _playerInteraction = GetComponent<PlayerInteractionController>();
        _playerCarryController = GetComponent<PlayerCarryController>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

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
        _timeSinceLastJump += Time.deltaTime;

        _currentState.UpdateStates();
    }

    public override void FixedUpdate()
    {
        if (_playerInteraction.IsInteracting()) { return; }

        StayInShip();

        if (_canMove)
        {
            CustomCollisionDecection();
            Move();
            RotateTowardsMove();
            AnimateMove();
            ApplyGravity();
        }

        //CheckIfFellOutOfShip();
    }

    public void OnDestroy()
    {
        if (_playerInput == null) { return; }

        _playerInput.OnJump -= HandleJump;
    }

    #endregion

    private void StayInShip()
    {
        if (!_isAttachedToShip) { return; }

        float offsetAmount = 3.5f;

        if (transform.localPosition.x > 7.5f) { transform.localPosition = new Vector3(transform.localPosition.x - offsetAmount, transform.localPosition.y, transform.localPosition.z); }
        else if (transform.localPosition.x < -7.5f) { transform.localPosition = new Vector3(transform.localPosition.x + offsetAmount, transform.localPosition.y, transform.localPosition.z); }
        else if (transform.localPosition.y > 7.5f) { transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - offsetAmount, transform.localPosition.z); }
        else if (transform.localPosition.y < -7.5f) { transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + offsetAmount, transform.localPosition.z); }
    }

    private void CustomCollisionDecection()
    {
        float height = _capsuleCollider.height;
        float width = 0.5f;
        float feetClippingHeightRatio = 8f;
        float feetRaycastLength = height / feetClippingHeightRatio;
        Vector3 midPoint = _capsuleCollider.height / 2f * Vector3.up;
        Vector3 feetMidPoint = transform.position + _capsuleCollider.height / feetClippingHeightRatio * Vector3.up;
        bool bothFeetOnFloor = true;
        _currentBlockedDirection = Vector3.zero;

        //draw 2 raycasts per side
        for (int i = -1; i < 2; i++)
        {
            if(i == 0) { continue; }

            Vector3 RaycastDrawPosition = transform.position + midPoint + (i * midPoint / 2f);

            //check right
            if (Physics.Raycast(RaycastDrawPosition, Vector3.right, out RaycastHit hitR, width, _collisionLayers))
            {
                _currentBlockedDirection = Vector3.right;
            }

            //check left
            if (Physics.Raycast(RaycastDrawPosition, Vector3.left, out RaycastHit hitL, width, _collisionLayers))
            {
                _currentBlockedDirection = Vector3.left;
            }

            Vector3 feetRaycastPos = feetMidPoint + i * Vector3.right * (width/2f);

            //check down
            if (Physics.Raycast(feetRaycastPos, Vector3.down, out RaycastHit hit2D, feetRaycastLength, _collisionLayers))
            {
                bothFeetOnFloor = false;
            }
        }

        //check up
        if (Physics.Raycast(transform.position + _capsuleCollider.height * Vector3.up, Vector3.up, out RaycastHit hit2U, 0.15f, _collisionLayers))
        {
            if (_fallVelocity.y > 0f) { _fallVelocity = Vector3.zero; }
        }

        _applyGravity = bothFeetOnFloor;

        //Don't do diagonal checks if is not falling
        if (_applyGravity == false || _fallVelocity == Vector3.zero) { return; }

        float diagonalMoveAmount = _fallVelocity.magnitude * Time.deltaTime * 5f;

        //check diagonal down left
        if (Physics.Raycast(feetMidPoint, new Vector3(-1f, -1f, 0f), out RaycastHit hitDL, feetRaycastLength * 1.5f, _collisionLayers))
        {
            transform.position += new Vector3(diagonalMoveAmount, diagonalMoveAmount, 0f);
        }

        //check diagonal down right
        if (Physics.Raycast(feetMidPoint, new Vector3(1f, -1f, 0f), out RaycastHit hitDR, feetRaycastLength * 1.5f, _collisionLayers))
        {
            transform.position += new Vector3(-diagonalMoveAmount, diagonalMoveAmount, 0f);
        }

        Debug.DrawRay(feetMidPoint, new Vector3(-1f, -1f, 0f) * feetRaycastLength * 2f, Color.cyan);
        Debug.DrawRay(feetMidPoint, new Vector3(1f, -1f, 0f) * feetRaycastLength * 2f, Color.cyan);
    }

    public override void Move()
    {
        Vector3 v3MoveInput = new Vector3(_playerInput.MoveDirection.x, 0f, _playerInput.MoveDirection.y);

        if(_currentBlockedDirection != Vector3.zero && v3MoveInput == _currentBlockedDirection) { return; }

        float cappedSpeed = _currentSpeed / 20;
        float zMovement = CameraManager.Instance.IsInOrthoMode ? 0f : _playerInput.MoveDirection.y * cappedSpeed;
        _moveDirection = new Vector3(_playerInput.MoveDirection.x * cappedSpeed, 0f, zMovement);

        //Caps players movement on Z axis
        if (!WalkPlaneVisual.Instance.IsWithinBounds(transform.position + _moveDirection)) { _moveDirection.z = 0; }

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
