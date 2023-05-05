//#define USE_DYNAMIC_ATTACHING

using UnityEngine;

public class MovingObjectAttacher : MonoBehaviour
{
    #region Private Variables

    private CharacterController _controller;
    private PlayerStateMachine _playerStateMachine;
    private Vector3 _moveDirection;
    private Vector3 _activeGlobalPlatformPoint;
    private Vector3 _activeLocalPlatformPoint;
    private Quaternion _activeGlobalPlatformRotation;
    private Quaternion _activeLocalPlatformRotation;

    #endregion

    #region Public Properties

    public Transform ActivePlatform;

    #endregion

    #region Unity Loops

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _playerStateMachine = GetComponent<PlayerStateMachine>();
    }

    private void LateUpdate()
    {
        Vector3 gravityVector = new Vector3(0f, _playerStateMachine.FallSpeed, 0f) * Time.deltaTime;
        Vector3 movementVector = _playerStateMachine.MoveDirection * Time.deltaTime;
        Debug.Log("playerStateMachine.MoveDirection: " + _playerStateMachine.MoveDirection);
        if (ActivePlatform != null)
        {
            Vector3 newGlobalPlatformPoint = ActivePlatform.TransformPoint(_activeLocalPlatformPoint);
            _moveDirection = newGlobalPlatformPoint - _activeGlobalPlatformPoint;
            //if (_moveDirection.magnitude > 0.01f)
            //{
                _controller.Move(_moveDirection + movementVector + gravityVector);
           // }
            if (ActivePlatform)
            {
                // Support moving platform rotation
                Quaternion newGlobalPlatformRotation = ActivePlatform.rotation * _activeLocalPlatformRotation;
                Quaternion rotationDiff = newGlobalPlatformRotation * Quaternion.Inverse(_activeGlobalPlatformRotation);

                // Prevent rotation of the local up vector
                rotationDiff = Quaternion.FromToRotation(rotationDiff * Vector3.up, Vector3.up) * rotationDiff;
                transform.rotation = rotationDiff * transform.rotation;
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                UpdateMovingPlatform();
            }
        }
        //else
        //{
        //    if (_moveDirection.magnitude > 0.01f)
        //    {
        //        _moveDirection = Vector3.Lerp(_moveDirection, Vector3.zero, Time.deltaTime);
        //        _controller.Move(_moveDirection + movementVector + gravityVector); 
        //    }
        //}
    }

    #endregion

    public void SetActivePlatform(Transform newPlatform)
    {
        ActivePlatform = newPlatform;
    }

    private void UpdateMovingPlatform()
    {
        _activeGlobalPlatformPoint = transform.position;
        _activeLocalPlatformPoint = ActivePlatform.InverseTransformPoint(transform.position);

        // Support moving platform rotation
        _activeGlobalPlatformRotation = transform.rotation;
        _activeLocalPlatformRotation = Quaternion.Inverse(ActivePlatform.rotation) * transform.rotation;
    }

#if USE_DYNAMIC_ATTACHING
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Make sure we are really standing on a straight platform
        // Not on the underside of one and not falling down from it either!
        if (hit.moveDirection.y < -0.9 && hit.normal.y > 0.41)
        {
            if (ActivePlatform != hit.collider.transform)
            {
                ActivePlatform = hit.collider.transform;
                UpdateMovingPlatform();
            }
        }
        else
        {
            ActivePlatform = null;
        }
    }

#endif
}
