using AwesomeTechnologies.VegetationSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetationStudioCameraAdder : MonoBehaviour
{
    #region Private Variables

    private VegetationSystemPro _vsPro;

    #endregion

    private void Start()
    {
        _vsPro = GetComponent<VegetationSystemPro>();

        _vsPro.DisposeVegetationStudioCameras();

        _vsPro.AddCamera(ShipCamera.Instance.PerspectiveCamera);
    }
}
