using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class BuildingFarmUI : MonoBehaviour
{
    #region Editor Fields

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;
    [SerializeField] private TextMeshProUGUI _timerText;

    [Header("Panels & Transforms")]
    [SerializeField] private GameObject _buildingFarmPanel;
    [SerializeField] private Transform _resourcesContentTransform;
    [SerializeField] private FarmResourceItemUI _buildingFarmResourceUIPrefab;

    #endregion

    #region Private Variables

    private static BuildingFarmUI _instance;
    private List<FarmResourceItemUI> _resourceUIs = new List<FarmResourceItemUI>();

    #endregion

    #region Public Properties

    public static BuildingFarmUI Instance { get { return _instance; } }

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

    public void Initialize(BuildingFarmSO buildingFarmSO)
    {
        _itemNameText.text = buildingFarmSO.DisplayName;
        _itemDescriptionText.text = buildingFarmSO.Description;

        DestroyPrevListedItems();

        foreach (ItemQuantity resource in buildingFarmSO.ResourcesGenerated)
        {
            GameObject newResourceUI = Instantiate(_buildingFarmResourceUIPrefab.gameObject, _resourcesContentTransform);
            FarmResourceItemUI newResourceScript = newResourceUI.GetComponent<FarmResourceItemUI>();

            newResourceScript.Initialize(resource);

            _resourceUIs.Add(newResourceScript);
        }

        EnablePanel(true);
    }

    public void UpdateTimer(float remainingTime)
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = (int)remainingTime - (minutes * 60);

        _timerText.text = $"Next shipment in: {minutes}m{seconds}s";
    }

    public bool IsEnabled()
    {
        return _buildingFarmPanel.activeSelf;
    }

    public void EnablePanel(bool isEnabled)
    {
        _buildingFarmPanel.SetActive(isEnabled);
    }

    private void DestroyPrevListedItems()
    {
        _resourceUIs.Clear();

        foreach (Transform child in _resourcesContentTransform)
        {
            if(child == _resourcesContentTransform) { continue; }

            Destroy(child.gameObject);
        }
    }
}
