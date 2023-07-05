using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class GameAudioManager : MonoBehaviour
{
    #region Editor Fields

    [Header("Interactable Sounds")]
    [SerializeField] private EventReference _boosterBoost;

    [Header("Combat Sounds")]
    [SerializeField] private EventReference _stunnedSfx;
    [SerializeField] private EventReference _ragdollCollision;
    [SerializeField] private EventReference _punchImpactSfx;
    [SerializeField] private EventReference _punchSwingSfx;

    [Header("Special Monster Sounds")]

    #endregion

    #region Private Variables

    private static GameAudioManager _instance;

    #endregion

    #region Public Properties

    public static GameAudioManager Instance { get { return _instance; } }

    public EventReference BoosterBoost => _boosterBoost;
    public EventReference StunnedSfx => _stunnedSfx;
    public EventReference RagdollCollision => _ragdollCollision;
    public EventReference PunchSwingSfx => _punchSwingSfx;
    public EventReference PunchImpactSfx => _punchImpactSfx;

    #endregion

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void PlaySound(EventReference sound, Vector3 position)
    {
        FMODUnity.RuntimeManager.PlayOneShot(sound, position);
    }
}
