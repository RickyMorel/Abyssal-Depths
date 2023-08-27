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

        if (!BasePartsManager.Instance.HasNextLocation()) { return; }

        StartCoroutine(TransformToBasePart());
    }

    private IEnumerator TransformToBasePart()
    {
        GameAudioManager.Instance.PlaySound(GameAudioManager.Instance.TransformToBasePart, transform.position);

        yield return new WaitForSeconds(1f);

        BasePart basePart = Instantiate(GameAssetsManager.Instance.BasePart).GetComponent<BasePart>();
        basePart.Initialize(gameObject, _basePartType);
    }
}
