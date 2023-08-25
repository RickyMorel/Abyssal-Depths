using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingFarm : BuildingUpgradable
{
    #region Editor Fields

    [SerializeField] private BuildingFarmSO _buildingFarmSO;

    #endregion

    #region Private Varaibles

    private float _timeTillNextBatch;

    #endregion

    #region Unity Loops

    private void Update()
    {
        _timeTillNextBatch += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        
    }

    #endregion

    public override void Interact()
    {
        if (!CanUse()) { return; }

        base.Interact();

        BuildingFarmUI.Instance.Initialize(_buildingFarmSO);
    }

    public override void Uninteract()
    {
        base.Uninteract();

        BuildingFarmUI.Instance.EnablePanel(false);
    }
}
