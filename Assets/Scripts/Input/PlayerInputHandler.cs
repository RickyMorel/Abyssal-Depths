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
    private bool _isShooting;
    private bool _canPlayerSpawn = false;

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
    public event Action<PlayerInputHandler> OnTrySpawn;
   
    public Vector2 MoveDirection => _moveDirection;
   
    public bool IsShooting => _isShooting;

    #endregion

    #region Getters And Setters

    public bool CanPlayerSpawn { get { return _canPlayerSpawn; } set { _canPlayerSpawn = value; } }
    public int PlayerId { get { return _playerId; } set { _playerId = value; } }
    public Player PlayerInputs { get { return _player; } set { _player = value; } }

    #endregion

    private void Start()
    {
        _player = ReInput.players.GetPlayer(_playerId);

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
    }

    public void Move()
    {
        if (!IsPlayerActive) { return; }

        float moveHorizontal = _player.GetAxisRaw("Horizontal");
        float moveVertical = _player.GetAxisRaw("Vertical");

        _moveDirection = new Vector2(moveHorizontal, moveVertical).normalized;
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
}