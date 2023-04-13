using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSaber : MeleeWeapon
{
    #region Editor Fields

    [SerializeField] private GameObject _handle;
    [SerializeField] private BoxCollider _bladeBoxCollider;

    #endregion

    #region Private Variables

    private float _rotationSpeed = 20;
    private bool _isBladeOut = false;

    #endregion

    #region Unity Loops

    public override void Start()
    {
        base.Start();

        _bladeBoxCollider.enabled = false;
    }

    public override void FixedUpdate()
    {
        if (_weapon.CurrentPlayer != null && !_isBladeOut) 
        {
            _isBladeOut = true;
            TimelinesManager.Instance.LightSaberBlade.gameObject.transform.SetParent(_handle.transform);
            TimelinesManager.Instance.LightSaberBlade.gameObject.transform.localPosition = new Vector3(0, 0, 0);
            TimelinesManager.Instance.LightSaberOut.Play();
            _bladeBoxCollider.enabled = true;
        }
        else if (_weapon.CurrentPlayer == null && _isBladeOut) 
        {
            _isBladeOut = false;
            TimelinesManager.Instance.LightSaberBlade.gameObject.transform.localPosition = new Vector3(0, 0, 0);
            TimelinesManager.Instance.LightSaberIn.Play();
            _bladeBoxCollider.enabled = false;
        }
    }

    #endregion

    public override void Shoot()
    {
        _handle.transform.Rotate(_rotationSpeed * Time.deltaTime, 0, 0);
    }

    public override void CheckShootInput()
    {
        if (_weapon.CurrentPlayer.IsUsing)
        {
            Shoot();
        }
    }
}