using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Volume))]
public class VolumeInterface : MonoBehaviour
{
    #region Private Variables

    private static VolumeInterface _instance;
    private Volume _volume;

    private Vignette _vignette;

    private float _originalVignetteIntensity;

    #endregion

    #region Public Properties
    public static VolumeInterface Instance { get { return _instance; } }

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
        _volume = GetComponent<Volume>();
        
        if(_volume.profile.TryGet<Vignette>(out Vignette vignette)) { _vignette = vignette; }



        _originalVignetteIntensity = _vignette.intensity.value;
    }

    public void ChangeVignetteByPercentage(float newIntensityPercentage)
    {
        _vignette.intensity.Override(newIntensityPercentage);
    }

    public void ResetVignette()
    {
        _vignette.intensity.Override(_originalVignetteIntensity);
    }
}
