using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingFarm : BuildingUpgradable
{
    #region Editor Fields

    [SerializeField] private BuildingFarmSO _buildingFarmSO;

    #endregion

    public override void Interact()
    {
        Debug.Log("Interact: " + gameObject.name);

        base.Interact();

        BuildingFarmUI.Instance.Initialize(_buildingFarmSO);
    }

    public override void Uninteract()
    {
        base.Uninteract();

        BuildingFarmUI.Instance.EnablePanel(false);
    }
}
