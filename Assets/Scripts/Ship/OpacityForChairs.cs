using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpacityForChairs : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private Interactable _interactable;
    [SerializeField] private MeshRenderer _chairMaterial;

    #endregion

    #region Private Variables

    private float _timer = 0;

    #endregion

    #region Unity Loops

    private void Start()
    {
        _chairMaterial.material.color = new Color(_chairMaterial.material.color.r, _chairMaterial.material.color.g, _chairMaterial.material.color.b, 1);
    }

    private void Update()
    {
        if (_interactable.CurrentPlayer == null && _chairMaterial.material.color.a == 1) { return; }

        if (_interactable.CurrentPlayer != null && _chairMaterial.material.color.a > 0.5f)
        {
            _timer += Time.deltaTime;
            _chairMaterial.material.color = new Color(_chairMaterial.material.color.r, _chairMaterial.material.color.g, _chairMaterial.material.color.b, Mathf.Lerp(1, 0.5f, _timer)); 
        }

        if (_interactable.CurrentPlayer == null && _chairMaterial.material.color.a < 1)
        {
            if (_timer > 1) { _timer = 1; }

            _timer -= Time.deltaTime;
            this._chairMaterial.material.color = new Color(_chairMaterial.material.color.r, _chairMaterial.material.color.g, _chairMaterial.material.color.b, Mathf.Lerp(1, 0.5f, _timer)); 
        }
    }

    #endregion
}
