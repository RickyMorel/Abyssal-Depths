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

    private void OnEnable()
    {
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
            _fogText.text = "You survived The Fog!";
            _playableDirector.Play();
        }
    }
}
