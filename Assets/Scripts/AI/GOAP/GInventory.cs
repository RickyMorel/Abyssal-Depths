using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GInventory
{
    public List<GameObject> Items = new List<GameObject>();

    public void AddItem(GameObject item)
    {
        Items.Add(item);
    }

    public GameObject FindItemWithTag(string tag)
    {
        foreach (GameObject item in Items)
        {
            if (item.tag == tag)
                return item;
        }
        return null;
    }

    public void RemoveItem(GameObject itemToRemove)
    {
        int indexToRemove = -1;

        foreach (GameObject item in Items)
        {
            indexToRemove++;
            if (item == itemToRemove)
                break;
        }

        if (indexToRemove > -1)
            Items.RemoveAt(indexToRemove);
    }
}
