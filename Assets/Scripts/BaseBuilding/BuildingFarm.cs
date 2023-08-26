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

    /// <summary>
    /// Generates resources and adds to inventory every x amount of time after being repaired
    /// </summary>
    private void FixedUpdate()
    {
        if (!CanUse()) { return; }

        _timeTillNextBatch += Time.deltaTime;

        UpdateTimerUI();

        if (_timeTillNextBatch < _buildingFarmSO.GenerationMinutes * 60f) { return; }

        _timeTillNextBatch = 0f;

        MainInventory.Instance.AddItems(_buildingFarmSO.ResourcesGenerated, false);
    }

    #endregion

    private void UpdateTimerUI()
    {
        if (!BuildingFarmUI.Instance.IsEnabled()) { return; }

        float remainingTime = (_buildingFarmSO.GenerationMinutes * 60f) - _timeTillNextBatch;

        BuildingFarmUI.Instance.UpdateTimer(remainingTime);
    }

    public override bool Interact()
    {
        Debug.Log("Interact: " + CanUse());
        if (!CanUse()) { return false; }

        base.Interact();

        BuildingFarmUI.Instance.Initialize(_buildingFarmSO);

        return true;
    }

    public override void Uninteract()
    {
        base.Uninteract();

        BuildingFarmUI.Instance.EnablePanel(false);
    }
}
