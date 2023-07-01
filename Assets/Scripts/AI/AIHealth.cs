using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DynamicMeshCutter;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class AIHealth : PlayerHealth
{
    #region Editor Fields

    [SerializeField] private bool _canKill = false;
    [SerializeField] private bool _stopsActionWhenHurt = true;
    [SerializeField] private bool _isBoss = false;

    #endregion

    #region Private Variables

    private GAgent _gAgent;
    private AIInteractionController _interactionController;
    private Rigidbody _rb;
    private MeshTarget _meshTarget;

    #endregion

    #region Public Properties

    public static event Action<int> OnBossDied;

    public bool CanKill => _canKill;
    public bool IsBoss => _isBoss;

    #endregion

    #region Unity Loops

    public override void Start()
    {
        base.Start();

        _gAgent = GetComponent<GAgent>();
        _interactionController = GetComponent<AIInteractionController>();
        _rb = GetComponent<Rigidbody>();

        _meshTarget = GetComponentInChildren<MeshTarget>();
        if(_meshTarget != null) { _meshTarget.enabled = false; }

        OnDamaged += Hurt;
    }

    private void OnDestroy()
    {
        OnDamaged -= Hurt;
    }

    #endregion

    public override void Damage(int damage, bool isImpactDamage = false, bool isDamageChain = false, Collider instigatorCollider = null, int index = 0)
    {
        base.Damage(damage, isImpactDamage, isDamageChain, instigatorCollider, index);

        StartCoroutine(PushWhenShot());
    }

    private IEnumerator PushWhenShot()
    {
        _gAgent.StateMachine.CanMove = false;
        _rb.isKinematic = false;

        _rb.AddForce(-transform.forward * 40, ForceMode.VelocityChange);

        yield return new WaitForSeconds(1f);

        _gAgent.StateMachine.CanMove = true;
        _rb.isKinematic = true;
    }

    public override void Die()
    {
        base.Die();

        CutMeshIfLightSaber();

        StopPreviousAction();
        _gAgent.enabled = false;
        _rb.isKinematic = false;
        _rb.useGravity = true;

        if (_isBoss) { InvokeBossDiedEvent(); }

        Invoke(nameof(DisableSelf), 5f);
    }

    private void CutMeshIfLightSaber()
    {
        if (_damageData == null) { return; }

        if (!(_damageData.DamageTypes[0] == DamageTypes.Base && _damageData.DamageTypes[1] == DamageTypes.Laser) || !(_damageData.DamageTypes[1] == DamageTypes.Laser && _damageData.DamageTypes[0] == DamageTypes.Base)) { return; }
        if (_meshTarget != null) { _meshTarget.enabled = true; }
        GetComponentInChildren<PlaneBehaviour>().Cut();

        Invoke(nameof(DisableSelf), 0.1f);
    }

    public override void Hurt(DamageTypes damageType, int damage)
    {
        if (CanKill == false) { base.Hurt(damageType, damage); }

        CheckIfLowHealth();
        CheckIfScared();
    }

    private void CheckIfScared()
    {
        if (_gAgent.Beliefs.HasState("scared")) { return; }

        if (_stopsActionWhenHurt) { StopPreviousAction(); }

        _gAgent.Beliefs.AddState("scared", 1);
    }

    private void CheckIfLowHealth()
    {
        //If health is below 50%
        if (CurrentHealth > MaxHealth * 0.5f) { return; }

        if (_gAgent.Beliefs.HasState("hurt")) { return; }

        if (_stopsActionWhenHurt) { StopPreviousAction(); }

        _gAgent.Beliefs.AddState("hurt", 1);
    }

    private void StopPreviousAction()
    {
        _gAgent.CancelPreviousActions();
        _interactionController.CheckExitInteraction();
    }

    private void DisableSelf()
    {
        gameObject.SetActive(false);
    }

    public void InvokeBossDiedEvent()
    {
        OnBossDied?.Invoke(SceneManager.GetActiveScene().buildIndex);
    }
}