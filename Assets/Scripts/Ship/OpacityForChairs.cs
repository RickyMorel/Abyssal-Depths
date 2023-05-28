using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpacityForChairs : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private Upgradable _upgradable;
    [SerializeField] private float _transparencyChangeSpeed;

    #endregion

    #region Private Variables

    private Color _meshRenderer;

    #endregion

    #region Unity Loops

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>().material.color;
        _meshRenderer.a = 255;
    }

    private void Update()
    {
        if (_upgradable.CurrentPlayer == null && _meshRenderer.a >= 255) { Debug.Log("1"); return; }

        if (_upgradable.CurrentPlayer != null && _meshRenderer.a < 128)
        {
            Debug.Log("2");
            _meshRenderer.a = Mathf.Lerp(_meshRenderer.a, 128, Time.deltaTime * _transparencyChangeSpeed);
        }
        else if (_upgradable.CurrentPlayer == null && _meshRenderer.a < 255)
        {
            Debug.Log(_upgradable.name + " " +"3");
            _meshRenderer.a = Mathf.Lerp(_meshRenderer.a, 255, Time.deltaTime * _transparencyChangeSpeed);
        }
    }

    #endregion
}
