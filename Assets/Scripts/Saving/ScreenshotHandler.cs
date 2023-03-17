using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Rendering;
using static SaveFileScreenshotDemo;

public class ScreenshotHandler : MonoBehaviour
{
    private static ScreenshotHandler _instance;
    private Camera _camera;
    private bool _takeScreenshotOnNextFrame;
    private Action<Texture2D> onScreenshotTaken;

    private void Awake()
    {
        _instance = this;
        _camera = gameObject.GetComponent<Camera>();
        _camera.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Take Screenshot");
            TakeScreenshotStatic(1080, 1080, (Texture2D screenshotTexture) =>
            {

            });
        }
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

        _camera.enabled = true;

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

    private void TakeScreenshot(int width, int height, Action<Texture2D> onScreenShotTaken)
    {
        _camera.targetTexture = RenderTexture.GetTemporary(width, height);
        _takeScreenshotOnNextFrame = true;
        this.onScreenshotTaken = onScreenShotTaken;
    }

    public static void TakeScreenshotStatic(int width, int height, Action<Texture2D> onScreenShotTaken)
    {
        _instance.TakeScreenshot(width, height, onScreenShotTaken);
    }
}
