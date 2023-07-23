using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    #region Private Variables

    private PlayerStateMachine _playerStateMachine;

    #endregion

    private void Start()
    {
        _playerStateMachine = GetComponent<PlayerStateMachine>();
    }

    //Called by animation event
    public void PlayFootStepSfx()
    {
        if(_playerStateMachine.CurrentState is not PlayerGroundedState) { return; }

        GameAudioManager.Instance.PlaySound(GameAudioManager.Instance.MetalFootSteps, transform.position);
    }
}