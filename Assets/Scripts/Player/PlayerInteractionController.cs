
using System;
using UnityEngine;

public class PlayerInteractionController : BaseInteractionController
{
    #region Private Variables

    private CharacterController _characterController;
    private PlayerStateMachine _playerStateMachine;
    private Rigidbody _rb;

    #endregion

    #region Unity Loops

    public override void Start()
    {
        base.Start();

        _playerInput = GetComponent<PlayerInputHandler>();
        _characterController = GetComponent<CharacterController>();
        _rb = GetComponent<Rigidbody>();
        _playerStateMachine = GetComponent<PlayerStateMachine>();

        _playerInput.OnInteract += PlayerHandleInteraction;
        _playerInput.OnJump += HandleJump;
        _playerHealth.OnHurt += HandleHurt;
        OnExitInteraction += HandleExitInteraction;
    }

    public override void Update()
    {
        base.Update();

        MoveDirection = _playerInput.InteractableMoveDirection;
        IsUsing = _playerInput.IsShooting;
        IsUsing_2 = _playerInput.IsShooting_2;
        _characterController.enabled = CurrentInteraction == 0;
        _rb.isKinematic = CurrentInteraction != 0;
    }

    private void OnDestroy()
    {
        _playerInput.OnInteract -= PlayerHandleInteraction;
        _playerInput.OnJump -= HandleJump;
        _playerHealth.OnHurt -= HandleHurt;
        OnExitInteraction -= HandleExitInteraction;
    }

    public void PlayerHandleInteraction()
    {
        HandleInteraction();
    }

    private void HandleExitInteraction()
    {
        _playerStateMachine.FallSpeed = Physics.gravity.y;
    }

    #endregion
}
