using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMovement : MonoBehaviour
{
    #region Editor Fields

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

        Debug.Log("HandleCycleChange");

        if (!BasePartsManager.Instance.HasNextLocation()) { return; }

        Debug.Log("BasePartsManager has next location");

        StartCoroutine(TransformToBasePart());
    }

    private IEnumerator TransformToBasePart()
    {
        Debug.Log("TransformToBasePart");

        GameAudioManager.Instance.PlaySound(GameAudioManager.Instance.TransformToBasePart, transform.position);

        yield return new WaitForSeconds(1f);

        BasePart basePart = Instantiate(GameAssetsManager.Instance.BasePart).GetComponent<BasePart>();
        basePart.Initialize(gameObject, _basePartType);
    }
}