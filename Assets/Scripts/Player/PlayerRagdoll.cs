using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerRagdoll : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private List<Collider> _colliders = new List<Collider>();
    [SerializeField] private Transform _stunParticlesTransform;

    #endregion

    #region Private Variables

    private Collider _mainCollider;
    private Animator _anim;
    private NavMeshAgent _agent;
    private Rigidbody _rb;
    private PlayerHealth _health;
    private ParticleSystem _bubbleParticles;
    private GameObject _stunnedParticleInstance;

    private bool _isKinematicInitialState;
    private bool _useGravityInitialState;
    private bool _agentEnabledInitialState;
    #endregion

    #region Public Properties

    public Rigidbody Hips => _colliders[0].attachedRigidbody;

    #endregion

    #region Unity Loops

    private void Awake()
    {
        _mainCollider = GetComponent<Collider>();
        _anim = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
        _health = GetComponent<PlayerHealth>();
    }

    private void Start()
    {
        _bubbleParticles = Instantiate(GameAssetsManager.Instance.RagdollBubbleParticles, transform).GetComponent<ParticleSystem>();

        if(_rb != null)
        {
            _isKinematicInitialState = _rb.isKinematic;
            _useGravityInitialState = _rb.useGravity;
            _agentEnabledInitialState = _agent.enabled;
        }

        LockZPos();
        EnableDeadRagdoll(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(_rb == null) { return; }

        if(LayerMask.LayerToName(collision.gameObject.layer) == "ShieldLayer") { return; }

        _health.DamageWithoutDamageData((int)_health.CalculateCrashDamage(_rb, 1f));
    }

    #endregion

    private void LockZPos()
    {
        foreach (Collider collider in _colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezePositionZ;
        }
    }

    public void EnableLivingRagdoll(bool isEnabled)
    {
        if (isEnabled)
        {
            _rb.isKinematic = false;
            _rb.useGravity = true;
            _agent.enabled = false;
            _mainCollider.isTrigger = false;
            _bubbleParticles.Play();
            EnableStunFX(true);
        }
        else
        {
            _rb.isKinematic = _isKinematicInitialState;
            _rb.useGravity = _useGravityInitialState;
            _agent.enabled = _agentEnabledInitialState;
            _mainCollider.isTrigger = true;
            _bubbleParticles.Stop();
            EnableStunFX(false);
        }
    }

    public void EnableStunFX(bool enable)
    {
        Debug.Log("EnableStunFX: " + enable + " " + gameObject.name);
        if (enable)
        {
            Transform wantedTransform = GetHeadTransform() != null ? GetHeadTransform() : transform;

            if (wantedTransform == null) { return; }

            Debug.Log("Spawned stun particles: " + gameObject.name);

            //Play initial hit particles
            GameObject stunHitParticles = Instantiate(GameAssetsManager.Instance.StunnedParticles[0], wantedTransform.position, wantedTransform.rotation);
            Debug.Log("stunHitParticles: " + stunHitParticles);

            if (SceneLoader.IsInGarageScene() == false) { stunHitParticles.transform.localScale *= 3f; }

            //Play looping stun particles
            if (_stunnedParticleInstance != null) { return; }

            _stunnedParticleInstance = Instantiate(GameAssetsManager.Instance.StunnedParticles[1], wantedTransform);
            _stunnedParticleInstance.transform.localScale = _stunParticlesTransform.localScale;
        }
        else
        {
            if (_stunnedParticleInstance == null) { return; }

            _stunnedParticleInstance.GetComponent<ParticleSystem>().Stop();
        }
    }

    public Transform GetHeadTransform()
    {
        foreach (Collider collider in _colliders)
        {
            if (!collider.gameObject.name.Contains("Head")) { continue; }

            return collider.transform;
        }

        return _stunParticlesTransform;
    }

    public void EnableDeadRagdoll(bool isEnabled)
    {
        if (isEnabled) _bubbleParticles.Play(); else _bubbleParticles.Stop();

        if (_colliders.Count <= 1) { if (isEnabled) { PlayDeathAnimation(); } return; }

        if (isEnabled == false)
        {
            transform.position = _colliders[0].transform.position;
        }

        DisableMovement(isEnabled, true);

        foreach (Collider collider in _colliders)
        {
            collider.enabled = isEnabled;
        }

        if(!_health.IsDead()) { EnableStunFX(isEnabled); }
    }

    private void DisableMovement(bool isEnabled, bool hasRagdoll)
    {
        _mainCollider.enabled = !isEnabled;
        if (hasRagdoll) { _anim.enabled = !isEnabled; }
        if (_agent != null) { _agent.enabled = !isEnabled; }
    }

    private void PlayDeathAnimation()
    {
        _anim.SetBool("IsDead", true);
        DisableMovement(true, false);
    }
}
