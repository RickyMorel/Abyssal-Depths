using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Damageable
{
    #region Editor Fields

    [SerializeField] private float _hurtTime = 5f;
    [SerializeField] private Renderer _mesh;

    #endregion

    #region Private Variables

    private bool _isHurt;

    #endregion

    #region Public Properties

    public bool IsHurt { get { return _isHurt; } set { _isHurt = value; } }

    public event Action OnHurt;

    #endregion

    private void Awake()
    {
        _mesh = GetComponentInChildren<MeshRenderer>();
        if (_mesh == null) { _mesh = GetComponentInChildren<SkinnedMeshRenderer>(); }
    }

    //This is for the child classes
    public virtual void Start()
    {
        base.Start();
    }

    public virtual void Hurt(DamageType damageType)
    {
        OnHurt?.Invoke();

        StartCoroutine(HurtCoroutine());
    }

    private IEnumerator HurtCoroutine()
    {
        _isHurt = true;

        yield return new WaitForSeconds(_hurtTime);

        _isHurt = false;
    }

    public override void UpdateHealthUI()
    {
        if(_mesh == null) { Debug.LogError("Need to assign mesh with blood shader!: " + gameObject.name); return; }

        _mesh.material.SetFloat("Blood", 1 - (CurrentHealth / MaxHealth));
    }
}
