using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Rendering;

public class ScreenshotHandler : MonoBehaviour
{
    #region Private Variables

    private static ScreenshotHandler _instance;
    private Camera _camera;
    private bool _takeScreenshotOnNextFrame;
    private Action<Texture2D> onScreenshotTaken;

    #endregion

    #region Public Properties

    public static ScreenshotHandler Instance { get { return _instance; } }

    #endregion

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

    private void Start()
    {
        _camera = gameObject.GetComponent<Camera>();
        _camera.enabled = false;
    }

    private void OnEnable()
    {
        RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
    }

    private void OnDisable()
    {
        RenderPipelineManager.endCameraRendering -= RenderPipelineManager_endCameraRendering;
    }

    private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext arg1, Camera arg2)
    {
        if (!_takeScreenshotOnNextFrame) { return; }

        _takeScreenshotOnNextFrame = false;

        RenderTexture renderTexture = _camera.targetTexture;

        Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
        renderResult.ReadPixels(rect, 0, 0);

        byte[] byteArray = renderResult.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/SaveScreenshot.png", byteArray);
        Debug.Log("Screenshot Saved!");

        RenderTexture.ReleaseTemporary(renderTexture);
        _camera.targetTexture = null;

        onScreenshotTaken(renderResult);
        onScreenshotTaken = null;

        _camera.enabled = false;
    }

    public void TakeScreenshot(int width, int height, Action<Texture2D> onScreenShotTaken)
    {
        _camera.targetTexture = RenderTexture.GetTemporary(width, height);
        _takeScreenshotOnNextFrame = true;
        this.onScreenshotTaken = onScreenShotTaken;
        _camera.enabled = true;
    }
}
