using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minable : Lootable
{
    #region Editor Fields

    //The impact force the pickaxe needs to damage the minable object
    [SerializeField] private float _damageForce;
    [SerializeField] private float _maxHealth;
    [SerializeField] private ParticleSystem _damageParticles;
    [SerializeField] private EventReference _impactSfx;

    #endregion

    #region Private Fields

    private float _currentHealth;
    private MeshRenderer _mesh;

    #endregion

    #region Unity Loops

    private void Start()
    {
        _currentHealth = _maxHealth;

        if(_damageParticles == null) { InstantiateRockParticles(); }
    }

    #endregion

    private void InstantiateRockParticles()
    {
        _mesh = GetComponentInChildren<MeshRenderer>();

        GameObject rockParticlesInstance = Instantiate(GameAssetsManager.Instance.MinableHitDefault, transform);
        ParticleSystem particlesScript = rockParticlesInstance.GetComponent<ParticleSystem>();
        ParticleSystemRenderer particlesRenderer = particlesScript.GetComponent<ParticleSystemRenderer>();
        particlesRenderer.material = _mesh.material;

        //Set to default layer so you can see it in Perspective Camera
        rockParticlesInstance.layer = 0;
        rockParticlesInstance.transform.localScale *= 2f;
        rockParticlesInstance.transform.localPosition = Vector3.zero;

        _damageParticles = particlesScript;
    }

    public void Damage(float impactForce)
    {
        if(impactForce < _damageForce) { return; }

        float damage = impactForce - _damageForce;

        _currentHealth -= damage;

        DamagePopup.Create(transform.position, (int)damage, DamageTypes.None, true);

        _damageParticles.Play();

        GameAudioManager.Instance.PlaySound(_impactSfx, transform.position);

        CheckIfBreak();
    }

    private void CheckIfBreak()
    {
        if(_currentHealth > 0) { return; }

        _damageParticles.transform.parent = null;

        ShipInventory shipInventory = FindObjectOfType<ShipInventory>();

        Loot(shipInventory);
    }
}
