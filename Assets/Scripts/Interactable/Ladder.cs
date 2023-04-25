using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.TryGetComponent(out PlayerStateMachine playerStateMachine)) { return; }

        playerStateMachine.UseLadder();
    }
}
