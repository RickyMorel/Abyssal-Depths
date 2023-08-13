using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BuildingUpgradeUI : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject _buildingUpgradeRequirementsPanel;

    [SerializeField] private Transform _requirementsContentTransform;

    #endregion

    #region Private Variables

    private static BuildingUpgradeUI _instance;

    #endregion

    #region Public Properties

    public static BuildingUpgradeUI Instance { get { return _instance; } }

    #endregion

    #region Unity Loops

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

    #endregion

    public void EnableUpgradesPanel(bool isEnabled)
    {
        InventoryUI.Instance.EnableInventory(isEnabled);
        _buildingUpgradeRequirementsPanel.SetActive(isEnabled);
    }

    private void DestroyPrevListedItems()
    {
        foreach (Transform child in _requirementsContentTransform)
        {
            if(child == _requirementsContentTransform) { continue; }

            Destroy(child.gameObject);
        }
    }
}
