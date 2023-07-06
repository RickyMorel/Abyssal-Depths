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

    [Header("Interactable Sounds")]
    [SerializeField] private EventReference _boosterBoost;

    [Header("Combat Sounds")]
    [SerializeField] private EventReference _stunnedSfx;
    [SerializeField] private EventReference _ragdollCollision;
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

    #endregion

    #region Private Variables

    private static GameAudioManager _instance;

    #endregion

    #region Public Properties

    public static GameAudioManager Instance { get { return _instance; } }

    public EventReference FastTravelGo => _fastTravelGo;
    public EventReference FastTravelArrive => _fastTravelArrive;
    public EventReference PortalGateOpening => _portalGateOpening;
    public EventReference PortalGateShaking => _portalGateShaking;
    public EventReference PortalGateInteraction => _portalGateInteraction;
    public EventReference BoosterBoost => _boosterBoost;
    public EventReference StunnedSfx => _stunnedSfx;
    public EventReference RagdollCollision => _ragdollCollision;
    public EventReference PunchSwingSfx => _punchSwingSfx;
    public EventReference PunchImpactSfx => _punchImpactSfx;
    public EventReference ShipSirenSlowSfx => _shipSirenSlowSfx;
    public EventReference ShipSirenFastSfx => _shipSirenFastSfx;
    public EventReference CraftingTableCrafting => _craftingTableCrafting;
    public EventReference CraftingTableCrafted => _craftingTableCrafted;
    public EventReference CraftingTableInteract => _craftingTableInteract;

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
}