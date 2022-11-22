using UnityEngine;

[RequireComponent(typeof(BaseInteractionController))]
[RequireComponent(typeof(PlayerRagdoll))]
[RequireComponent(typeof(PlayerHealth))]
public class BaseStateMachine : MonoBehaviour
{
    #region Editor Fields

    [Header("Movement")]
    [SerializeField] protected float _currentSpeed;
    [SerializeField] protected float _runSpeed;

    [Header("Rotation")]
    [SerializeField] protected float _turnSmoothTime;

    [Header("Jumping")]
    [SerializeField] protected float _jumpHeight = 3f;
    [SerializeField] protected float _gravityIntensity = -15f;
    [SerializeField] protected bool _isGrounded;

    [Header("State Variables")]
    [SerializeField] protected PlayerBaseState _currentState;
    [SerializeField] protected PlayerStateFactory _states;

    #endregion

    #region Getters & Setters

    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public float Speed { get { return _currentSpeed; } set { _currentSpeed = value; } }
    public float JumpHeight { get { return _jumpHeight; } set { _jumpHeight = value; } }
    public float GravityIntensity { get { return _gravityIntensity; } set { _gravityIntensity = value; } }
    public bool IsGrounded { get { return _isGrounded; } set { _isGrounded = value; } }
    public bool CanMove { get { return _canMove; } set { _canMove = value; } }

    #endregion

    #region Private Variables

    protected BaseInteractionController _playerInteraction;
    protected PlayerRagdoll _playerRagdoll;
    protected PlayerHealth _playerHealth;
    protected PlayerCarryController _playerCarryController;
    protected Rigidbody _rb = null;
    protected Animator _anim;

    protected bool _isJumpPressed;
    protected Vector3 _moveDirection;
    protected bool _canMove = true;

    #endregion

    #region Public Properties

    public BaseInteractionController PlayerInteraction => _playerInteraction;
    public PlayerRagdoll PlayerRagdoll => _playerRagdoll;
    public PlayerHealth PlayerHealth => _playerHealth;
    public PlayerCarryController PlayerCarryController => _playerCarryController;
    public Vector3 MoveDirection => _moveDirection;
    public Rigidbody Rb => _rb;
    public Animator Anim => _anim;
    public bool IsJumpPressed => _isJumpPressed;
    public float RunSpeed => _runSpeed;
    public bool IsShooting;

    #endregion

    #region Unity Loops

    public virtual void Awake()
    {
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
    }

    public virtual void Start()
    {
        _playerInteraction = GetComponent<BaseInteractionController>();
        _playerRagdoll = GetComponent<PlayerRagdoll>();
        _playerHealth = GetComponent<PlayerHealth>();
        _anim = GetComponent<Animator>();
    }

    public virtual void Update()
    {
        _currentState.UpdateStates();
    }

    public virtual void FixedUpdate()
    {
        if (_playerInteraction.IsInteracting()) { return; }

        if (_canMove)
        {
            Move();
            RotateTowardsMove();
            AnimateMove();
        }
    }

    #endregion

    public virtual void Move()
    {

    }

    public virtual void RotateTowardsMove()
    {

    }

    public virtual void AnimateMove()
    {
        _anim.SetFloat("Movement", _moveDirection.magnitude);
    }
}
