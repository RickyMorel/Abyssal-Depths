using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private LayerMask _floorLayers;
    [SerializeField] private PlayerStateMachine _playerStateMachine;

    #endregion

    #region Public Properties

    public LayerMask FloorLayers => _floorLayers;

    #endregion

    private void OnTriggerStay(Collider other)
    {
        if (_floorLayers != (_floorLayers | (1 << other.gameObject.layer))) { return; }

        _playerStateMachine.IsGrounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (_floorLayers != (_floorLayers | (1 << other.gameObject.layer))) { return; }

        _playerStateMachine.IsGrounded = false;
    }
}