using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponents : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Animator _anim;
    [SerializeField] private PlayerInputHandler _playerInputHandler;
    [SerializeField] private PlayerInteractionController _playerInteractionController;
    [SerializeField] private PlayerCamera _playerCamera;

    #endregion

    #region Public Properties

    public Rigidbody Rb => _rb;
    public Animator Anim => _anim;
    public PlayerInputHandler PlayerInputHandler => _playerInputHandler;
    public PlayerInteractionController PlayerInteractionController => _playerInteractionController;
    public PlayerCamera PlayerCamera => _playerCamera;

    #endregion
}
