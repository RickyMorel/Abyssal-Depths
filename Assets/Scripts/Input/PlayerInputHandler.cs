using System;
using UnityEngine;
using Rewired;
using System.Linq;
using System.Collections;

public class PlayerInputHandler : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private int _playerId = 0;

    #endregion

    #region Private Variables

    private Player _player;

    private Vector2 _moveDirection;
    private Vector2 _interactableMoveDirection;
    private bool _isShooting;
    private bool _isShooting_2;
    private bool _canPlayerSpawn = false;

    private bool _doubleTapButtonWasPressed = false; // Flag to keep track of whether the button was pressed once or twice
    private float _timeSinceLastPress = 0f; // Keep track of the time since the last button press
    private float _doubleTapWindow = 0.25f; // The time window in which the button needs to be pressed twice

    #endregion

    #region Public Properties

    public bool IsPlayerActive = false;

    public static event Action<PlayerInputHandler, bool> OnSpecialAction;
    public static event Action<PlayerInputHandler> OnClick;

    public event Action OnJump;
    public event Action OnInteract;
    public event Action OnConfirm;
    public event Action OnCancel;
    public event Action OnUpgrade;
    public static event Action<PlayerInputHandler> OnChangeZoom;
    public event Action<PlayerInputHandler> OnTrySpawn;
   
    public Vector2 MoveDirection => _moveDirection;
    public Vector2 InteractableMoveDirection => _interactableMoveDirection;
   
    public bool IsShooting => _isShooting;
    public bool IsShooting_2 => _isShooting_2;

    #endregion

    #region Getters And Setters

    public bool CanPlayerSpawn { get { return _canPlayerSpawn; } set { _canPlayerSpawn = value; } }
    public int PlayerId { get { return _playerId; } set { _playerId = value; } }
    public Player PlayerInputs { get { return _player; } set { _player = value; } }

    #endregion

    private void Start()
    {
        _player = ReInput.players.GetPlayer(_playerId);

        DestroyDuplicatePlayers();

        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();

        //if finds a duplicate player, destroy self
        PlayerInputHandler[] playerInputs = FindObjectsOfType<PlayerInputHandler>();

        if (playerInputs.ToList().Find(x => (x.PlayerId == PlayerId) && (x.gameObject != gameObject))) { Destroy(transform.root.gameObject); }
    }

    private void Update()
    {
        Move();
        Jump();
        Confirm();
        Cancel();
        SpecialAction();
        Interact();
        Upgrade();
        Shoot();
        Shoot2();
        ChangeZoom();
    }

    private void DestroyDuplicatePlayers()
    {
        //Deletes duplicates of same player when loading new scenes
        PlayerInputHandler[] players = FindObjectsOfType<PlayerInputHandler>();

        foreach (PlayerInputHandler player in players)
        {
            //Don't compare to self
            if (player.gameObject == gameObject) { continue; }

            //Destroy self if finds another player with active id
            if (player.PlayerId == PlayerId) { Destroy(transform.root.gameObject); }
        }
    }

    public bool DetectDoubleTap()
    {
        bool doubleTapDetected = false;

        // Check if the button is pressed
        if (_player.GetButtonDown("Shoot2"))
        {
            // If it was pressed again within the time window, set the flag and return true
            if (_doubleTapButtonWasPressed && _timeSinceLastPress <= _doubleTapWindow)
            {
                doubleTapDetected = true;
            }
            // Otherwise, set the flag and reset the timer
            else
            {
                _doubleTapButtonWasPressed = true;
                _timeSinceLastPress = 0f;
            }
        }

        // Increment the timer
        if (_doubleTapButtonWasPressed)
        {
            _timeSinceLastPress += Time.deltaTime;
            if (_timeSinceLastPress > _doubleTapWindow)
            {
                _doubleTapButtonWasPressed = false;
            }
        }

        return doubleTapDetected;
    }

    public void Move()
    {
        if (!IsPlayerActive) { return; }

        float moveHorizontal = _player.GetAxisRaw("Horizontal");
        float moveVertical = _player.GetAxisRaw("Vertical");

        //If is inside ship, don't allow z movement
        float y = CameraManager.Instance.IsInOrthoMode ? 0f : moveVertical;

        _moveDirection = new Vector2(moveHorizontal, y).normalized;
        _interactableMoveDirection = new Vector2(moveHorizontal, moveVertical).normalized;
    }

    public void Jump()
    {
        if (!_player.GetButtonDown("Jump")) { return; }

        if (CanPlayerSpawn)
        {
            OnTrySpawn?.Invoke(this);
        }

        else if (IsPlayerActive)
        {
            OnJump?.Invoke();
        }
    }

    public void Confirm()
    {
        if (!IsPlayerActive) { return; }

        if (!_player.GetButtonDown("Confirm")) { return; }

        OnConfirm?.Invoke();
        OnClick?.Invoke(this);
    }

    public void Cancel()
    {
        if (!IsPlayerActive) { return; }

        if (!_player.GetButtonDown("Cancel")) { return; }

        OnCancel?.Invoke();
    }

    public void ChangeZoom()
    {
        if (!IsPlayerActive) { return; }

        if (!_player.GetButtonDown("ChangeZoom")) { return; }

        OnChangeZoom?.Invoke(this);
    }

    public void SpecialAction()
    {
        if (!IsPlayerActive) { return; }

        if (!_player.GetButtonDown("SpecialAction")) { return; }

        bool value = _player.GetButton("SpecialAction");
        OnSpecialAction?.Invoke(this, value);
    }

    public void Interact()
    {
        if (!IsPlayerActive) { return; }

        if (!_player.GetButtonDown("Interact")) { return; }

        OnInteract?.Invoke();
    }

    public void Upgrade()
    {
        if (!IsPlayerActive) { return; }

        if (!_player.GetButtonDown("Upgrade")) { return; }

        OnUpgrade?.Invoke();
    }

    public void Shoot()
    {
        if (!IsPlayerActive) { return; }

        _isShooting = _player.GetButton("Shoot");
    }

    public void Shoot2()
    {
        if (!IsPlayerActive) { return; }

        _isShooting_2 = _player.GetButton("Shoot2");
    }
}