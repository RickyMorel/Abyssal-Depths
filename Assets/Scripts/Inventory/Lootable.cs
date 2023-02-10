using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootable : MonoBehaviourID
{
    #region Editor Fields

    [SerializeField] private List<ItemQuantity> _loot = new List<ItemQuantity>();
    [SerializeField] private bool _canLoot = false;

    #endregion

    #region Unity Loops

    public virtual void OnTriggerEnter(Collider other)
    {
        if(_canLoot == false) { return; }

        if(_loot.Count < 1) { return; }

        if(!other.gameObject.TryGetComponent<ShipInventory>(out ShipInventory shipInventory)) { return; }

        Loot(shipInventory);
    }

    #endregion

    public void Loot(ShipInventory shipInventory)
    {
        shipInventory.AddItems(_loot);

        _loot.Clear();

        if (this is Minable) { gameObject.SetActive(false); }
        else { Destroy(gameObject); }
    }

    public void SetCanLoot(bool canLoot)
    {
        _canLoot = canLoot;
    }
}
