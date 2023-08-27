using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMovement : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private Transform _basePartSpawnPos;
    [SerializeField] private BasePartType _basePartType;

    #endregion

    #region Unity Loops

    private void Start()
    {
        DayNightManager.Instance.OnCycleChange += HandleCycleChange;
    }

    private void OnDestroy()
    {
        DayNightManager.Instance.OnCycleChange -= HandleCycleChange;
    }

    #endregion

    private void HandleCycleChange()
    {
        if (!DayNightManager.Instance.IsNightTime) { return; }

        Vector3 spawnPos = transform.position + Vector3.up * 3f;

        BasePart basePart = Instantiate(GameAssetsManager.Instance.BasePart, spawnPos, Quaternion.identity).GetComponent<BasePart>();
        basePart.Initialize(gameObject, _basePartType);
    }
}
