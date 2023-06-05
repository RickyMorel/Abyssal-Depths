using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CraftingStation : Interactable
{
    #region Editor Fields

    [SerializeField] private Transform _itemSpawnTransform;
    [SerializeField] private ParticleSystem _craftingParticle;
    [SerializeField] private PlayableDirector _craftingTimeline;
    [SerializeField] private  _craftingText

    #endregion

    #region Public Properties

    public static event Action OnCraft;

    #endregion

    public override void Awake()
    {
        base.Awake();

        Humble.OnInteract += HandleInteract;
        Humble.OnUninteract += HandleUnInteract;
    }

    private void OnDestroy()
    {
        Humble.OnInteract -= HandleInteract;
        Humble.OnUninteract -= HandleUnInteract;
    }

    public void TryCraft(CraftingRecipy craftingRecipy)
    {
        if (!CraftingManager.CanCraft(craftingRecipy)) { return; }

        StartCoroutine(PlayCraftingAnimation(craftingRecipy, craftingRecipy.CraftingIngredients));
    }

    private void Craft(CraftingRecipy craftingRecipy, List<ItemQuantity> usedResources)
    {
        MainInventory.Instance.RemoveItems(usedResources);

        GameObject spawnedItem = craftingRecipy.CraftedItem.Item.SpawnItemPickup(_itemSpawnTransform);
        if(spawnedItem.TryGetComponent<ChipPickup>(out ChipPickup chipPickup)) { chipPickup.Initialize(craftingRecipy.CraftedItem.Item); }

        OnCraft?.Invoke();

        RemoveCurrentPlayer();
    }

    private void HandleInteract()
    {
        if(CurrentPlayer == null) { HandleUnInteract(); return; }

        CraftingManager.Instance.EnableCanvas(true, CurrentPlayer.GetComponent<PlayerInputHandler>(), this);
    }

    private void HandleUnInteract()
    {
        CraftingManager.Instance.EnableCanvas(false, null, this);
    }

    private IEnumerator PlayCraftingAnimation(CraftingRecipy craftingRecipy, List<ItemQuantity> usedResources)
    {
        _craftingTimeline.Play();

        yield return new WaitForSeconds(1.2f);

        _craftingParticle.Play();
        Craft(craftingRecipy, usedResources);
    }
}