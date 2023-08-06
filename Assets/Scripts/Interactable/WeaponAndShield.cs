using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAndShield : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private Weapon _weapon;
    [SerializeField] private ShieldWheel _shieldWheel;

    #endregion

    #region Unity Loops

    private void Start()
    {
        _weapon.RotateOnUpdate = false;
        _shieldWheel.RotateOnUpdate = false;
        _shieldWheel.CanUse = false;
    }

    private void Update()
    {
        if(_weapon.CurrentPlayer == null) { return; }

        if(_weapon.CanUse == false) { return; }

        if (_shieldWheel.CurrentPlayer != _weapon.CurrentPlayer) { _shieldWheel.SetCurrentPlayer(_weapon.CurrentPlayer); }

        _weapon.Rotate(1);
        _shieldWheel.RotateWASD(2);
    }

    #endregion
}
