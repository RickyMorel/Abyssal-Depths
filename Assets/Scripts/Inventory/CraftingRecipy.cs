using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/CraftingRecipy", order = 2)]
public class CraftingRecipy : ScriptableObject
{
    public ItemQuantity CraftedItem;
    public List<ItemQuantity> CraftingIngredients = new List<ItemQuantity>();
    public bool IsRepair = false;
}

#region Helper Classes

[System.Serializable]
public class ItemQuantity
{
    public Item Item;
    public int Amount;

    public ItemQuantity(Item item, int amount)
    {
        Item = item;
        Amount = amount;
    }

    public ItemQuantity()
    {

    }
}

#endregion
