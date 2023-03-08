using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class InteractableHealth : Damageable
{
    #region Editor Fields

    [SerializeField] private CraftingRecipy _fixCost;
    [SerializeField] private float _timeToFix = 8f;
    [SerializeField] private bool _canUseWhenBroken = false;
    [SerializeField] private Transform _particleSpawnTransform;

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

    #endregion

    #region Unity Loops

    public override void Awake()
    {
        base.Awake();

        _interactable = GetComponent<Interactable>();

        _interactable.OnInteract += TryStartFix;
        _interactable.OnUninteract += TryStopFix;
    }

    public override void Start()
    {
        base.Start();

        _originalOutlineColor = _interactable.Outline.OutlineColor;
    }

    private void OnDestroy()
    {
        _interactable.OnInteract -= TryStartFix;
        _interactable.OnUninteract -= TryStopFix;
    }

    private void Update()
    {
        if(_interactable.CurrentPlayer == null) { return; }

        if(_interactable.CurrentPlayer.IsFixing == false) { return; }

        if (!IsDead()) { return; }

        if (!CraftingManager.CanCraft(_fixCost)) { _interactable.RemoveCurrentPlayer(); return; }

        _timeSpentFixing += Time.deltaTime;

        if(_timeSpentFixing > _timeToFix) { FixInteractable(); }
    }

    public override void OnTriggerStay(Collider other)
    {
        base.OnTriggerStay(other);

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

    private void TryStartFix()
    {
        if (!IsDead()) { return; }

        if (_currentRepairPopup != null) { Destroy(_currentRepairPopup); }

        if (!CraftingManager.CanCraft(_fixCost)) { return; }

        if (_interactable.CurrentPlayer == null) { return; }

        if (_interactable.CurrentPlayer.IsFixing == false) { return; }

        _timeSpentFixing = 0f;

        _currentRepairPopup = RepairPopup.Create(transform, _particleSpawnTransform.localPosition, _timeToFix - _timeSpentFixing).gameObject;

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

        AddHealth((int)MaxHealth);

        _interactable.CanUse = true;
        _interactable.InteractionType = _prevInteractableState.InteractionType;
        _interactable.IsSingleUse = _prevInteractableState.IsSingleUse;
        _interactable.Outline.OutlineColor = _originalOutlineColor;
        _timeSpentFixing = 0f;
        _interactable.RemoveCurrentPlayer();
        if (usedItems) { MainInventory.Instance.RemoveItems(_fixCost.CraftingIngredients); }
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
