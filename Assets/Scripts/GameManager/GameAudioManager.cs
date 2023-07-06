using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;
using FMOD.Studio;

public class GameAudioManager : MonoBehaviour
{
    #region Editor Fields

    [Header("Ship Interactables Sounds")]
    [SerializeField] private EventReference _boosterBoostSfx;
    [SerializeField] private EventReference _attackMonitorBrokenSfx;
    [SerializeField] private EventReference _interactableAttackMonitorSfx;
    [SerializeField] private EventReference _shipDoorOpenCloseSfx;
    [SerializeField] private EventReference _fixingInteractableSfx;
    [SerializeField] private EventReference _fixedInteractableSfx;

    [Header("Combat Sounds")]
    [SerializeField] private EventReference _stunnedSfx;
    [SerializeField] private EventReference _ragdollCollisionSfx;
    [SerializeField] private EventReference _punchImpactSfx;
    [SerializeField] private EventReference _punchSwingSfx;

    [Header("Special Monster Sounds")]

    [Header("Ship Sounds")]
    [SerializeField] private EventReference _shipDamagedSirenSlowSfx;
    [SerializeField] private EventReference _shipDamagedSirenFastSfx;
    [SerializeField] private EventReference _shipCrashingSfx;

    [Header("Interactables")]
    [SerializeField] private EventReference _itemPickUpSfx;
    [SerializeField] private EventReference _itemDropSfx;
    [SerializeField] private EventReference _cryoPodOpen;

    #endregion

    #region Private Variables

    private static GameAudioManager _instance;

    #endregion

    #region Public Properties

    public static GameAudioManager Instance { get { return _instance; } }

    public EventReference BoosterBoostSfx => _boosterBoostSfx;
    public EventReference StunnedSfx => _stunnedSfx;
    public EventReference RagdollCollisionSfx => _ragdollCollisionSfx;
    public EventReference PunchSwingSfx => _punchSwingSfx;
    public EventReference PunchImpactSfx => _punchImpactSfx;
    public EventReference AttackMonitorBrokenSfx => _attackMonitorBrokenSfx;
    public EventReference ShipDoorOpenCloseSfx => _shipDoorOpenCloseSfx;
    public EventReference ShipDamagedSirenSlowSfx => _shipDamagedSirenSlowSfx;
    public EventReference ShipDamagedSirenFastSfx => _shipDamagedSirenFastSfx;
    public EventReference ShipCrashingSfx => _shipCrashingSfx;
    public EventReference ItemPickUpSfx => _itemPickUpSfx;
    public EventReference ItemDropSfx => _itemDropSfx;
    public EventReference InteractableAttackMonitorSfx => _interactableAttackMonitorSfx;
    public EventReference FixingInteractableSfx => _fixingInteractableSfx;
    public EventReference FixedInteractableSfx => _fixedInteractableSfx;
    public EventReference CryoPodOpen => _cryoPodOpen;

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

    public EventInstance CreateSoundInstance(EventReference sound)
    {
        return FMODUnity.RuntimeManager.CreateInstance(sound);
    }

    public EventInstance CreateSoundInstance(EventReference sound, Transform objectTransform)
    {
        EventInstance eventInstance = FMODUnity.RuntimeManager.CreateInstance(sound);

        RuntimeManager.AttachInstanceToGameObject(eventInstance, objectTransform);

        return eventInstance;
    }
}