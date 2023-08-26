using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drill : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject _drillHead;
    [SerializeField] private float _drillRotationSpeed;
    [SerializeField] private Vector3 _rotation;

    #endregion

    #region Unity Loops

    private void Update()
    {
        DrillRotation();
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        
    }

    #endregion

    private void DrillRotation()
    {
        _drillHead.transform.Rotate(_rotation * Time.deltaTime * _drillRotationSpeed);
    }
}