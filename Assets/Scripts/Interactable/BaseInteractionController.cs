//#define INTERACTION_DEBUGS

using UnityEngine;
using UnityEngine.AI;

public class BaseInteractionController : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] protected int _currentInteraction = 0;

    #endregion

    #region Private Variables

    protected PlayerHealth _playerHealth;
    protected PlayerInputHandler _playerInput;
    protected PlayerCarryController _playerCarryController;
    protected Animator _anim;
    protected Rigidbody _rb;
    private NavMeshAgent _agent;
    protected Interactable _currentInteractable;
    protected bool _canInteractInCurrentState = true;

    protected bool _isUsing = false;
    protected bool _isUsing_2 = false;
    protected bool _isFixing = false;
    protected Vector3 _moveDirection;
    protected float _timeSinceLastInteraction;

    #endregion

    #region Public Properties

    public PlayerInputHandler PlayerInput => _playerInput;
    public Interactable CurrentInteractable => _currentInteractable;

    #endregion

    #region Getters & Setters

    public bool IsUsing { get { return _isUsing; } set { _isUsing = value; } }
    public bool IsUsing_2 { get { return _isUsing_2; } set { _isUsing_2 = value; } }
    public bool IsFixing { get { return _isFixing; } set { _isFixing = value; } }
    public Vector3 MoveDirection { get { return _moveDirection; } set { _moveDirection = value; } }

    #endregion

    #region Unity Loops

    public virtual void Start()
    {
        _playerHealth = GetComponent<PlayerHealth>();
        _anim = GetComponent<Animator>();
        _playerCarryController = GetComponent<PlayerCarryController>();
        _rb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
    }

    public virtual void Update()
    {
        _timeSinceLastInteraction += Time.deltaTime;
    }

    #endregion

    public void HandleJump()
    {
        CheckExitInteraction();
    }

    public void HandleHurt()
    {
        CheckExitInteraction();
    }

    public virtual void HandleChangeState(PlayerBaseState newState, bool isRootState)
    {

    }

    public virtual void CheckExitInteraction()
    {
        //if is not doing interaction, return
        if (!IsInteracting()) { return; }

        SetInteraction(0, transform);

        _currentInteractable.Uninteract();
    }

    //This calls when the player presses the interact button
    public void HandleInteraction(float customDuration = -1)
    {
        if (_currentInteractable == null) { return; }

#if INTERACTION_DEBUGS
        Debug.Log("6-_currentInteractable != null: " + gameObject.name);

        Debug.Log($"6.5-{_currentInteractable.CurrentPlayer} == {this}: " + gameObject.name);
#endif
        //if (_currentInteractable.CurrentPlayer == this) { return; }
#if INTERACTION_DEBUGS
        Debug.Log($"7-{_currentInteractable.CurrentPlayer} == {this}: " + gameObject.name);
#endif

        //if you can't use the interactable while it's broken, return
        if (!_currentInteractable.CanUse && !IsFixing) { return; }
#if INTERACTION_DEBUGS
        Debug.Log("8-can use and is not fixing: " + gameObject.name);
#endif

        float singleUseDuration = customDuration == -1 ? _currentInteractable.SingleUseTime : customDuration;

        SetInteraction((int)_currentInteractable.InteractionType, _currentInteractable.PlayerPositionTransform);

        if (_playerCarryController != null) { _playerCarryController.DropAllItems(); }

        if (_currentInteractable.IsSingleUse) { Invoke(nameof(CheckExitInteraction), singleUseDuration); }
    }

    public virtual void SetCurrentInteractable(Interactable interactable)
    {
        _currentInteractable = interactable;
    }

    public bool IsInteracting()
    {
        return _currentInteraction != 0;
    }

    public bool HasRecentlyInteracted()
    {
        return _timeSinceLastInteraction < 0.25f;
    }

    public void SetInteraction(int interactionType, Transform playerPositionTransform)
    {
#if INTERACTION_DEBUGS
        Debug.Log($"SetInteraction: {gameObject.name}, " + interactionType);
#endif
        BaseInteractionController interactionController = interactionType == 0 ? null : this;

        _currentInteractable.SetCurrentPlayer(interactionController);
        _currentInteraction = interactionType;

        _anim.SetInteger("Interaction", interactionType);

        //if (CurrentInteractable.AttachOnInteract)
        //{
            if (_agent != null) { _agent.enabled = interactionType == 0; }
            transform.position = playerPositionTransform.position;
            transform.rotation = playerPositionTransform.rotation;
        //}

        _timeSinceLastInteraction = 0f;

        //Every time player uninteracts, set isFixing false
        if(interactionType == 0) { _isFixing = false; }
    }
}
