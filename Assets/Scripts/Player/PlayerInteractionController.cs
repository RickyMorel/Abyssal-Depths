
using UnityEngine;

public class PlayerInteractionController : BaseInteractionController
{
    #region Private Variables

    private CharacterController _characterController;

    #endregion

    #region Unity Loops

    public override void Start()
    {
        base.Start();

        _playerInput = GetComponent<PlayerInputHandler>();
        _characterController = GetComponent<CharacterController>();

        _playerInput.OnInteract += PlayerHandleInteraction;
        _playerInput.OnJump += HandleJump;
        _playerHealth.OnHurt += HandleHurt;
    }

    public override void Update()
    {
        base.Update();

        MoveDirection = _playerInput.InteractableMoveDirection;
        IsUsing = _playerInput.IsShooting;
        IsUsing_2 = _playerInput.IsShooting_2;
        _characterController.enabled = CurrentInteraction == 0;
    }

    private void OnDestroy()
    {
        _playerInput.OnInteract -= PlayerHandleInteraction;
        _playerInput.OnJump -= HandleJump;
        _playerHealth.OnHurt -= HandleHurt;
    }

    public void PlayerHandleInteraction()
    {
        HandleInteraction();
    }

    #endregion
}
