using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using RenderSettings = UnityEngine.RenderSettings;

public class DepthManager : MonoBehaviour
{
    #region Editor Fields

    [Header("Varaibles")]
    [SerializeField] private float _surfaceLevelDepth = -350f;
    [SerializeField] private float _seaAbyssDepth = -1000f;

    #endregion

    #region Private Variables

    private static DepthManager _instance;
    private float _currentDepth;

    #endregion

    #region Public Properties

    public static DepthManager Instance { get { return _instance; } }

    #endregion

    #region Unity Loops

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void OnGUI()
    {
        int seaLevel = 200;
        int depth = (int)Mathf.Abs(Ship.Instance.transform.position.y) - seaLevel;

        GameStatsPanelUI.Instance.UpdateDepth(depth);
    }

    #endregion

}