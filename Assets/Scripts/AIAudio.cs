using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAudio : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private EventReference _shootSfx;
    [SerializeField] private EventReference _dieSfx;

    #endregion

    public void PlayShootSFX()
    {
        GameAudioManager.Instance.PlaySound(_shootSfx, transform.position);
    }

    public void PlayDieSFX()
    {
        GameAudioManager.Instance.PlaySound(_dieSfx, transform.position);
    }
}
