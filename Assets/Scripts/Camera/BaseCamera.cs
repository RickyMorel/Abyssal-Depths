using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;
using System;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class BaseCamera : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] protected float _shakeAmplitude;
    [SerializeField] protected float _expandedFovToVelocityRatio = 0.06f;

    #endregion

    #region Private Variables

    protected CinemachineVirtualCamera _virtualCamera;
    protected CinemachineBasicMultiChannelPerlin _virtualCameraNoise;

    #endregion

    public virtual void Start()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _virtualCameraNoise = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float shakeAmplitude = 5f, float shakeFrecuency = 5f, float shakeTiming = 0.5f)
    {
        StartCoroutine(ProcessShake(shakeAmplitude, shakeFrecuency, shakeTiming));
    }

    private IEnumerator ProcessShake(float shakeAmplitude, float shakeFrecuency, float shakeTiming)
    {
        Debug.Log("ProcessShake");
        Noise(shakeAmplitude, shakeFrecuency);
        yield return new WaitForSeconds(shakeTiming);
        Noise(0, 0);
    }

    public void Noise(float amplitudeGain, float frequencyGain)
    {
        _virtualCameraNoise.m_AmplitudeGain = amplitudeGain;
        _virtualCameraNoise.m_FrequencyGain = frequencyGain;
    }
}

