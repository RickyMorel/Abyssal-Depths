using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FogTimeline : TimelineTrigger
{
    #region Editor Fields

    [SerializeField] private TextMeshProUGUI _fogText;

    #endregion

    #region Unity Loops

    public override void OnTriggerEnter(Collider other)
    {
        //Do nothing
    }

    public override void Start()
    {
        base.Start();

        EnemyWaveSystem.Instance.OnCycleChange += TheFogIsComing;
    }

    private void OnDisable()
    {
        EnemyWaveSystem.Instance.OnCycleChange -= TheFogIsComing;
    }

    #endregion

    private void TheFogIsComing()
    {
        if (EnemyWaveSystem.Instance.IsNightTime)
        {
            _fogText.text = "The Fog Is Coming";
            _playableDirector.Play();
        }
        else
        {
            _fogText.text = "Day " + EnemyWaveSystem.Instance.DayCount.ToString();
            _playableDirector.Play();
        }
    }
}