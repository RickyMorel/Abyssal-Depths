using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererAnimation : MonoBehaviour
{

    #region Editor Fields

    [SerializeField] private Texture[] _textures;
    [SerializeField] private float _fps = 30f;

    #endregion

    #region Private Variables

    private LineRenderer _lineRenderer;
    private int _animationStep;
    private float _fpsCounter;

    #endregion

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        _fpsCounter += Time.deltaTime;

        if(_fpsCounter >= 1f / _fps)
        {
            _animationStep++;
            if(_animationStep == _textures.Length)
                _animationStep = 0;

            _lineRenderer.material.SetTexture("_MainTex", _textures[_animationStep]);

            _fpsCounter = 0f;
        }
    }
}
