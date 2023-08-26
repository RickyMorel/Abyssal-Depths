using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;
using FMOD.Studio;
using Debug = UnityEngine.Debug;

public class GameAudioManager : MonoBehaviour
{
    #region Editor Fields

    [Header("Ship Interactables Sounds")]
    [SerializeField] private EventReference _boosterBoostSfx;
    [SerializeField] private EventReference _pickaxeImpactSfx;
    [SerializeField] private EventReference _pickaxeBoostSfx;
    [SerializeField] private EventReference _attackMonitorBrokenSfx;
    [SerializeField] private EventReference _interactableAttackMonitorSfx;
    [SerializeField] private EventReference _shipDoorOpenCloseSfx;
    [SerializeField] private EventReference _fixingInteractableSfx;
    [SerializeField] private EventReference _fixedInteractableSfx;
    [SerializeField] private EventReference _interactableOnlineSfx;
    [SerializeField] private EventReference _interactableOfflinefx;
    [SerializeField] private EventReference _weaponWheelRotationSfx;

    [Header("Combat Sounds")]
    [SerializeField] private EventReference _stunnedSfx;
    [SerializeField] private EventReference _ragdollCollisionSfx;
    [SerializeField] private EventReference _punchImpactSfx;
    [SerializeField] private EventReference _punchSwingSfx;

    [Header("Fast Travel Sounds")]
    [SerializeField] private EventReference _fastTravelGo;
    [SerializeField] private EventReference _fastTravelArrive;

    [Header("Gate Sounds")]
    [SerializeField] private EventReference _portalGateOpening;
    [SerializeField] private EventReference _portalGateShaking;
    [SerializeField] private EventReference _portalGateInteraction;

    [Header("Crafting Sounds")]
    [SerializeField] private EventReference _craftingTableCrafting;
    [SerializeField] private EventReference _craftingTableCrafted;
    [SerializeField] private EventReference _craftingTableInteract;

    [Header("Ship Sounds")]
    [SerializeField] private EventReference _shipSirenSlowSfx;
    [SerializeField] private EventReference _shipSirenFastSfx;

    [Header("Special Monster Sounds")]

    [Header("Player Sounds")]
    [SerializeField] private EventReference _metalFootsteps;

    [Header("Ship Sounds")]
    [SerializeField] private EventReference _shipDamagedSirenSlowSfx;
    [SerializeField] private EventReference _shipDamagedSirenFastSfx;
    [SerializeField] private EventReference _shipCrashingSfx;

    [Header("Interactables")]
    [SerializeField] private EventReference _itemPickUpSfx;
    [SerializeField] private EventReference _itemDropSfx;
    [SerializeField] private EventReference _cryoPodOpen;

    [Header("UI")]
    [SerializeField] private EventReference _fogIsComingSfx;
    [SerializeField] private EventReference _dayTransitionSfx;

    #endregion

    #region Private Variables

    private static GameAudioManager _instance;

    #endregion

    #region Public Properties

    public static GameAudioManager Instance { get { return _instance; } }

    public EventReference FastTravelGo => _fastTravelGo;
    public EventReference WeaponWheelRotationSfx => _weaponWheelRotationSfx;
    public EventReference FastTravelArrive => _fastTravelArrive;
    public EventReference PortalGateOpening => _portalGateOpening;
    public EventReference PortalGateShaking => _portalGateShaking;
    public EventReference PortalGateInteraction => _portalGateInteraction;
    public EventReference BoosterBoostSfx => _boosterBoostSfx;
    public EventReference StunnedSfx => _stunnedSfx;
    public EventReference RagdollCollisionSfx => _ragdollCollisionSfx;
    public EventReference PunchSwingSfx => _punchSwingSfx;
    public EventReference PunchImpactSfx => _punchImpactSfx;
    public EventReference ShipSirenSlowSfx => _shipSirenSlowSfx;
    public EventReference ShipSirenFastSfx => _shipSirenFastSfx;
    public EventReference CraftingTableCrafting => _craftingTableCrafting;
    public EventReference CraftingTableCrafted => _craftingTableCrafted;
    public EventReference CraftingTableInteract => _craftingTableInteract;
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
    public EventReference MetalFootSteps => _metalFootsteps;
    public EventReference InteractableOnlineSfx => _interactableOnlineSfx;
    public EventReference InteractableOfflinefx => _interactableOfflinefx;
    public EventReference PickaxeImpactSfx => _pickaxeImpactSfx;
    public EventReference PickaxeBoostSfx => _pickaxeBoostSfx;
    public EventReference FogIsComing => _fogIsComingSfx;
    public EventReference DayTransition => _dayTransitionSfx;

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

    public EventInstance CreateSoundInstance(EventReference sound, Transform objectTransform)
    {
        EventInstance eventInstance = FMODUnity.RuntimeManager.CreateInstance(sound);

        RuntimeManager.AttachInstanceToGameObject(eventInstance, objectTransform);

        return eventInstance;
    }

    public void AdjustAudioParameter(EventInstance audioInstance, string parameterName, float percentage)
    {
        audioInstance.setParameterByName(parameterName, percentage);
    }
}