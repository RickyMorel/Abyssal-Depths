using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactable
{
    #region Editor Fields

    [SerializeField] private Transform _itemSpawnTransform;

    #endregion

    #region Unity Loops

    private void Start()
    {
        OnInteract += HandleInteract;
        OnUninteract += HandleUninteract;
    }

    private void OnDestroy()
    {
        OnInteract -= HandleInteract;
        OnUninteract -= HandleUninteract;
    }

    #endregion

    private void HandleInteract()
    {
        PlayerInteractionController playerInteractionController = _currentPlayer as PlayerInteractionController;

        if(playerInteractionController == null) { return; }

        MainInventory.Instance.EnableInventory(true, this, playerInteractionController.PlayerInput);
    }

    private void HandleUninteract()
    {
        MainInventory.Instance.EnableInventory(false, null, null);
    }

    public void SpawnItem(ItemQuantity itemQuantity)
    {
        for (int i = 0; i < itemQuantity.Amount; i++)
        {
            GameObject itemInstance = Instantiate(itemQuantity.Item.ItemPrefab, _itemSpawnTransform.position, Quaternion.identity);
        }

        MainInventory.Instance.RemoveItem(itemQuantity);
    }

    public void StoreItem(Collider other)
    {
        if (!other.gameObject.TryGetComponent<PlayerCarryController>(out PlayerCarryController playerCarryController)) { return; }

        StartCoroutine(StoreItemCoroutine(playerCarryController));
    }

    //Prevents from player dropping items while entering trigger, and duplication glitches
    private IEnumerator StoreItemCoroutine(PlayerCarryController playerCarryController)
    {
        yield return new WaitForSeconds(0.5f);

        foreach (ItemPickup itemPrefab in playerCarryController.ItemsCarrying)
        {
            ItemQuantity itemQuantity = new ItemQuantity(itemPrefab.ItemSO, 1);
            MainInventory.Instance.AddItem(itemQuantity);
        }

        playerCarryController.DestroyAllItems();
    }
}
