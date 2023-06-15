using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class RepairPopup : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private TextMeshPro _timerText;

    #endregion

    #region Private Variables

    private float _repairTime;
    private bool _isStatePopup = false;

    #endregion

    #region Public Properties

    public static RepairPopup Create(Transform interatableTransform, Vector3 localPos, float repairDuration)
    {
        GameObject repairPopupObj = Instantiate(GameAssetsManager.Instance.RepairPopup, interatableTransform);
        repairPopupObj.transform.localPosition = new Vector3(localPos.x, localPos.y, localPos.z);

        RepairPopup repairPopup = repairPopupObj.GetComponent<RepairPopup>();
        repairPopup.Setup(repairDuration);

        return repairPopup;
    }

    public static RepairPopup CreateStatePopup(Transform interatableTransform, Vector3 localPos, int currentParts,int maxParts)
    {
        GameObject repairPopupObj = Instantiate(GameAssetsManager.Instance.RepairPopup, interatableTransform);
        repairPopupObj.transform.localPosition = new Vector3(localPos.x, localPos.y+1f, localPos.z);

        RepairPopup repairPopup = repairPopupObj.GetComponent<RepairPopup>();
        repairPopup.SetupStatePopup(currentParts, maxParts);

        repairPopupObj.transform.localScale = repairPopupObj.transform.localScale / 1.5f;

        return repairPopup;
    }

    #endregion

    private void Awake()
    {
        _timerText = GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        if (_isStatePopup) { return; }

        UpdateRepairTime();
    }

    private void UpdateRepairTime()
    {
        _repairTime = Mathf.Clamp(_repairTime - Time.deltaTime, 0f, 999f);
        int repairTimeInt = (int)_repairTime;
        _timerText.text = repairTimeInt.ToString();

        if (_repairTime <= 1f) { Destroy(gameObject); }
    }

    public void Setup(float repairTime)
    {
        _repairTime = repairTime + 1f;
        _timerText.fontSize = _timerText.fontSize * 0.7f;
    }

    public void SetupStatePopup(int currentParts,int maxParts)
    {
        _timerText.text = $"{currentParts}/{maxParts}";

        _isStatePopup = true;
    }
}
