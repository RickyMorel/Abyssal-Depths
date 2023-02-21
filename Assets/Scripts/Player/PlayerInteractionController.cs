
using UnityEngine;

public class PlayerInteractionController : BaseInteractionController
{
    #region Unity Loops

    public override void Start()
    {
        base.Start();

        _playerInput = GetComponent<PlayerInputHandler>();

        _playerInput.OnInteract += PlayerHandleInteraction;
        _playerInput.OnJump += HandleJump;
        _playerHealth.OnHurt += HandleHurt;
    }

    public override void Update()
    {
        base.Update();

        MoveDirection = _playerInput.MoveDirection;
        IsUsing = _playerInput.IsShooting;
        IsUsing_2 = _playerInput.IsShooting_2;
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
