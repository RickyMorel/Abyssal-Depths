using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using Random = UnityEngine.Random;

public class InteractableHealth : Damageable
{
    #region Editor Fields

    [SerializeField] private CraftingRecipy _fixCost;
    [SerializeField] private float _timeToFix = 8f;
    [SerializeField] private bool _canUseWhenBroken = false;
    [SerializeField] private Transform _particleSpawnTransform;

    [Header("Fix Parts Variables")]
    [SerializeField] private int _fixPartSpawnAmount = 2;
    [SerializeField] private GameObject _fixPartPickupPrefab;

    #endregion

    #region Public Properties

    public event Action OnFix;

    #endregion

    #region Private Variables

    private Interactable _interactable;
    private GameObject _currentParticles;
    private float _timeSpentFixing = 0f;
    private PrevInteractableState _prevInteractableState;
    private Color _originalOutlineColor;
    private GameObject _currentRepairPopup;
    private GameObject _currentRepairCanvas;
    private GameObject _currentRepairStatePopup;
    private int _partLeftToRepair = 0;

    #endregion

    #region Unity Loops

    public override void Awake()
    {
        base.Awake();

        _interactable = GetComponent<Interactable>();

        _interactable.Humble.OnInteract += TryStartFix;
        _interactable.Humble.OnUninteract += TryStopFix;
        ShipHealth.OnRespawn += HandleRespawn;
    }

    public override void Start()
    {
        base.Start();

        _originalOutlineColor = _interactable.Outline.OutlineColor;
    }

    private void OnDestroy()
    {
        _interactable.Humble.OnInteract -= TryStartFix;
        _interactable.Humble.OnUninteract -= TryStopFix;
        ShipHealth.OnRespawn -= HandleRespawn;
    }

    private void Update()
    {
        if(_interactable.CurrentPlayer == null) { return; }

        if(_interactable.CurrentPlayer.IsFixing == false) { return; }

        if (!IsDead()) { return; }

        _timeSpentFixing += Time.deltaTime;

        if(_timeSpentFixing > _timeToFix) { FixInteractable(); }
    }

    public override void OnCollisionStay(Collision collision)
    {
        base.OnCollisionStay(collision);

        if(_interactable.Outline.enabled == false) { return; }

        if(_currentRepairCanvas != null) { return; }

        if (!IsDead()) { return; }

        _currentRepairCanvas = RepairCostUI.Create(transform, _interactable.PlayerPositionTransform.localPosition, _fixCost).gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (_interactable.Outline.enabled == true) { return; }

        if(_currentRepairCanvas != null) { Destroy(_currentRepairCanvas); }
    }

    #endregion

    private void HandleRespawn()
    {
        FixInteractable(false);
    }

    private void TryStartFix()
    {
        Debug.Log("TryStartFix");
        if (!IsDead()) { return; }

        Debug.Log("Is dead");

        if (_currentRepairPopup != null) { Destroy(_currentRepairPopup); }

        if (_interactable.CurrentPlayer == null) { return; }

        Debug.Log("CurrentPlayer is not null");

        if (_interactable.CurrentPlayer.IsFixing == false) { return; }

        Debug.Log("CurrentPlayer is fixing");

        _timeSpentFixing = 0f;

        _currentRepairPopup = RepairPopup.Create(transform, _particleSpawnTransform.localPosition + new Vector3(0f, -0.8f, 0f), _timeToFix - _timeSpentFixing).gameObject;

        _interactable.CanUse = false;
    }

    private void TryStopFix()
    {
        if (_currentRepairPopup != null) { Destroy(_currentRepairPopup); }

        if (_canUseWhenBroken) { _interactable.CanUse = true; }
    }

    public void FixInteractable(bool usedItems = true)
    {
        if(_prevInteractableState == null) { return; }

        if(usedItems) 
        {
            _interactable.CurrentPlayer.GetComponent<PlayerUpgradesController>().DestroyHeldFixPart();
            _partLeftToRepair--;

            CreateRepairStatePopup();
            _interactable.RemoveCurrentPlayer();

            //Stay broken until player adds all parts
            if (_partLeftToRepair > 0) { return; }
        }

        AddHealth((int)MaxHealth);

        CreateRepairStatePopup(true);

        _interactable.CanUse = true;
        _interactable.InteractionType = _prevInteractableState.InteractionType;
        _interactable.IsSingleUse = _prevInteractableState.IsSingleUse;
        _interactable.Outline.OutlineColor = _originalOutlineColor;
        _timeSpentFixing = 0f;
        OnFix?.Invoke();

        DestroyAllParticles();

        if (_currentRepairCanvas != null) { Destroy(_currentRepairCanvas); }
    }

    public void DestroyAllParticles()
    {
        foreach (Transform child in _particleSpawnTransform)
        {
            if(child == _particleSpawnTransform) { continue; }

            Destroy(child.gameObject);
        }
    }

    public override void Die()
    {
        base.Die();

        _currentParticles = Instantiate(Ship.Instance.ShipStatsSO.InteractableFriedParticles.gameObject, transform);
        _currentParticles.transform.localPosition = new Vector3(
            _particleSpawnTransform.localPosition.x,
            _particleSpawnTransform.localPosition.y,
            _particleSpawnTransform.localPosition.z);

        _currentParticles.transform.parent = _particleSpawnTransform;

        if (_canUseWhenBroken == false) 
        {
            _interactable.CanUse = false; 
            _interactable.RemoveCurrentPlayer();
        }
        else 
        {
            _interactable.CanUse = true;
        }

        _prevInteractableState = new PrevInteractableState(_interactable.InteractionType, _interactable.IsSingleUse, _interactable.Outline.OutlineColor);
        _interactable.InteractionType = InteractionType.Fixing;
        _interactable.IsSingleUse = false;
        _interactable.Outline.OutlineColor = Color.red;

        SpawnFixParts();
    }

    private void SpawnFixParts()
    {
        for (int i = 0; i < _fixPartSpawnAmount; i++)
        {
            GameObject fixPartPickupInstance = Instantiate(_fixPartPickupPrefab, _particleSpawnTransform.position, Quaternion.identity);

            Rigidbody rb = fixPartPickupInstance.GetComponent<Rigidbody>();

            Vector3 forceDir = Random.insideUnitSphere * 50f;

            rb.AddForce(forceDir, ForceMode.Impulse);

            _partLeftToRepair++;
        }

        CreateRepairStatePopup();
    }

    private void CreateRepairStatePopup(bool justDestroy = false)
    {
        if(_currentRepairStatePopup != null) 
        {
            Destroy(_currentRepairStatePopup.gameObject);

            if (justDestroy) { return; }
        }

        _currentRepairStatePopup = RepairPopup.CreateStatePopup(transform, _particleSpawnTransform.localPosition,
            _fixPartSpawnAmount - _partLeftToRepair, _fixPartSpawnAmount).gameObject;
    }

    #region Helper Classes

    private class PrevInteractableState
    {
        public PrevInteractableState(InteractionType originalInteractionType, bool originalIsSingleUse, Color outlineColor)
        {
            InteractionType = originalInteractionType;
            IsSingleUse = originalIsSingleUse;
            OutlineColor = outlineColor;
        }

        public InteractionType InteractionType;
        public bool IsSingleUse;
        public Color OutlineColor;
    }

    #endregion
}
