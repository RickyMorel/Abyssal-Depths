using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BossDiedPanelUI : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private PlayableDirector _bossDiedPanelTimeline;

    #endregion

    private void Start()
    {
        AIHealth.OnBossDied += PlayBossDiedTimeline;
    }

    private void OnDestroy()
    {
        AIHealth.OnBossDied -= PlayBossDiedTimeline;
    }

    public void PlayBossDiedTimeline(int sceneBuildIndex)
    {
        _bossDiedPanelTimeline.Play();
    }
}
