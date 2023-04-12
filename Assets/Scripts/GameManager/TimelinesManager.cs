using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelinesManager : MonoBehaviour
{
    #region Editor Fields

    [Header("Ship Fast Travel")]
    [SerializeField] private PlayableDirector _startFastTravelTimeline;
    [SerializeField] private PlayableDirector _endFastTravelTimeline;
    [SerializeField] private PlayableDirector _cameraFadeTimeline;

    [SerializeField] private ParticleSystem _blackHoleParticle;

    [SerializeField] private GameObject _mainShipParentForTheTimeline;

    [Header("LightSaber")]
    [SerializeField] private PlayableDirector _lightSaberOut;
    [SerializeField] private PlayableDirector _lightSaberIn;

    [SerializeField] private GameObject _lightSaberBlade;

    #endregion

    #region Private Variables

    private static TimelinesManager _instance;

    #endregion

    #region Public Properties

    public static TimelinesManager Instance { get { return _instance; } }

    public PlayableDirector StartFastTravelTimeline => _startFastTravelTimeline;
    public PlayableDirector EndFastTravelTimeline => _endFastTravelTimeline;
    public PlayableDirector CameraFadeTimeline => _cameraFadeTimeline;

    public ParticleSystem BlackHoleParticle => _blackHoleParticle;

    public GameObject MainShipParentForTheTimeline => _mainShipParentForTheTimeline;

    public PlayableDirector LightSaberOut => _lightSaberOut;
    public PlayableDirector LightSaberIn => _lightSaberIn;

    public GameObject LightSaberBlade => _lightSaberBlade;

    #endregion

    #region Unity Loops

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

    #endregion
}