using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateMonitor : MonoBehaviour
{
    private int _avgFrameRate;
    private GUIStyle _style;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Update()
    {
        float current = 0;
        current = (int)(1f / Time.unscaledDeltaTime);
        _avgFrameRate = (int)current;
    }

    void InitStyles()
    {
        _style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleLeft,
            margin = new RectOffset(),
            padding = new RectOffset(),
            fontSize = 20,
            fontStyle = FontStyle.Bold
        };
    }

    private void OnGUI()
    {
        if (_style == null)
            InitStyles();

        GUI.Label(new Rect(10, 10, 1000, 100), $"FPS: {_avgFrameRate}", _style);
    }
}
