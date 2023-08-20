using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(StudioEventEmitter))]
public class ParticleAudio : MonoBehaviour
{
    #region Private Variables

    private StudioEventEmitter _particlePlaySfx;
    private ParticleSystem _particleSystem;

    #endregion

    private void Start()
    {
        _particlePlaySfx = GetComponent<StudioEventEmitter>();
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void LateUpdate()
    {
        if (_particleSystem.isPlaying && !_particlePlaySfx.IsPlaying())
        {
            _particlePlaySfx.Play();
        }
        else if( _particleSystem.isStopped && _particlePlaySfx.IsPlaying())
        {
            _particlePlaySfx.Stop();
        }
    }

}