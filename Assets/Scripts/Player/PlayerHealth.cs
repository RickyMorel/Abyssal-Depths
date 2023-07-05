using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Damageable
{
    #region Editor Fields

    [SerializeField] private float _hurtTime = 5f;
    [SerializeField] private Renderer[] _meshes;
    [SerializeField] protected Transform _particleParentTransform;
    [SerializeField] private ParticleSystem _bloodParticles;

    #endregion

    #region Private Variables

    private bool _isHurt;

    #endregion

    #region Public Properties

    public bool IsHurt { get { return _isHurt; } set { _isHurt = value; } }

    public event Action OnHurt;

    #endregion

    public override void Awake()
    {
        base.Awake();

        _meshes = GetComponentsInChildren<MeshRenderer>();
        if (_meshes.Length < 1) { _meshes = GetComponentsInChildren<SkinnedMeshRenderer>(); }
    }

    //This is for the child classes
    public override void Start()
    {
        base.Start();

        OnDamaged += UpdateBloodFX;
        OnDamagedWithPosition += PlayBloodParticles;
        OnUpdateHealth += HandleUpdateHealth;
    }

    private void OnDestroy()
    {
        OnDamaged -= UpdateBloodFX;
        OnUpdateHealth -= HandleUpdateHealth;
        OnDamagedWithPosition -= PlayBloodParticles;
    }

    public virtual void Hurt(DamageTypes damageType, int damage)
    {
        OnHurt?.Invoke();

        StartCoroutine(HurtCoroutine(_hurtTime));
    }

    public virtual void Hurt(DamageTypes damageType, int damage, float customHurtTime)
    {
        OnHurt?.Invoke();

        StartCoroutine(HurtCoroutine(customHurtTime));
    }

    private IEnumerator HurtCoroutine(float hurtTime)
    {
        _isHurt = true;

        yield return new WaitForSeconds(hurtTime);

        _isHurt = false;
    }

    private void HandleUpdateHealth(int damage)
    {
        UpdateBloodFX(DamageTypes.None, damage);
    }

    public void UpdateBloodFX(DamageTypes damageType, int damage)
    {
        if(_meshes.Length < 1) { Debug.LogError("Need to assign mesh with blood shader!: " + gameObject.name); return; }

        float bloodAmount = (1 - (CurrentHealth / MaxHealth))/3;

        foreach (var mesh in _meshes)
        {
            mesh.material.SetFloat("Blood", bloodAmount);
        }
    }

    public void PlayBloodParticles(Vector3 hurtPosition)
    {
        if(_bloodParticles == null) { return; }

        _bloodParticles.transform.position = hurtPosition;
        _bloodParticles.Play();
    }

    public override void Die()
    {
        base.Die();
    }

    private IEnumerator DissolveCoroutine()
    {
        yield return new WaitForSeconds(2f);

        float currentTime = 0f;
        float duration = 1f;

        while (currentTime <= duration)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / duration;
            float value = Mathf.Lerp(1, 0, t);

            foreach (var mesh in _meshes)
            {
                mesh.material.SetFloat("Dissolve", 1 - value);
            }

            yield return null;
        }
    }
}
