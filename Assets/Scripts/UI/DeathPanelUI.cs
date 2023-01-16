using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DeathPanelUI : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private PlayableDirector _deathPanelTimeline;

    #endregion

    #region Private Variables
    
    private static DeathPanelUI _instance;

    #endregion

    #region Public Properties
    public static DeathPanelUI Instance { get { return _instance; } }

    public PlayableDirector DeathPanelTimeLine => _deathPanelTimeline;

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

    public void PlayDeathTimeline()
    {
        _deathPanelTimeline.Play();
    }
}
