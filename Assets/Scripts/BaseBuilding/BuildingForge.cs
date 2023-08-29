using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingForge : BuildingUpgradable
{
    private void Craft(CraftingRecipy craftingRecipy, List<ItemQuantity> usedResources)
    {
        GameAudioManager.Instance.PlaySound(GameAudioManager.Instance.CraftingTableCrafted, transform.position);

        MainInventory.Instance.RemoveItems(usedResources);

        GameObject spawnedItem = craftingRecipy.CraftedItem.Item.SpawnItemPickup(ShipMovingStaticManager.Instance.ShipStaticObj.transform);

        //If item is not a pickup, then add material to inventory
        if (spawnedItem == null)
        {
            List<ItemQuantity> craftedItems = new List<ItemQuantity>();
            craftedItems.Add(craftingRecipy.CraftedItem);

            MainInventory.Instance.AddItems(craftedItems);
        }

        if (spawnedItem != null && spawnedItem.TryGetComponent(out ChipPickup chipPickup)) { chipPickup.Initialize(craftingRecipy.CraftedItem.Item); }
    }

    public override bool Interact()
    {
        if (!CanUse()) { return false; }

        base.Interact();

        CraftingManager.Instance.EnableCanvas(true, Ship.Instance.ShipLandingController.Booster.CurrentPlayer.PlayerInput);
        GameAudioManager.Instance.PlaySound(GameAudioManager.Instance.CraftingTableInteract, transform.position);

        return true;
    }

    public override void Uninteract()
    {
        base.Uninteract();

        CraftingManager.Instance.EnableCanvas(false, null);
    }
}
