using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainInventory : Inventory
{
    #region Private Variables

    private static MainInventory _instance;

    #endregion

    #region Public Properties

    public static MainInventory Instance { get { return _instance; } }

    #endregion

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
}
