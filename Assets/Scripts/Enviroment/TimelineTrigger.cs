using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(PlayableDirector))]
public class TimelineTrigger : MonoBehaviour
{
    #region Private Variables

    protected PlayableDirector _playableDirector;

    #endregion

    public virtual void Start()
    {
        _playableDirector = GetComponent<PlayableDirector>();
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.GetComponent<Ship>()) { return; }

        _playableDirector.Play();
    }
}
