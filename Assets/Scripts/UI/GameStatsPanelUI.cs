using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class GameStatsPanelUI : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private TextMeshProUGUI _daysText;
    [SerializeField] private TextMeshProUGUI _soulsText;
    [SerializeField] private TextMeshProUGUI _depthText;
    [SerializeField] private Image _baseHealthBar;

    #endregion

    #region Private Variables
    
    private static GameStatsPanelUI _instance;

    #endregion

    #region Public Properties
    public static GameStatsPanelUI Instance { get { return _instance; } }

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

    public void UpdateDays(int newDays)
    {
        _daysText.text = $"Day {newDays}";
    }

    public void UpdateDepth(int newDepth)
    {
        _depthText.text = $"{newDepth}m";
    }

    public void UpdateSouls(int newSouls)
    {
        _soulsText.text = $"{newSouls}";
    }

    public void UpdateBaseHealth(float currentHealth, float maxHealth)
    {
        _baseHealthBar.fillAmount = currentHealth / maxHealth;
    }
}
