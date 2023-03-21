using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;


public class LocationTrigger : TimelineTrigger
{
    #region Editor Fields

    [SerializeField] private string _locationName;
    [SerializeField] private TextMeshProUGUI _locationText;

    #endregion

    public override void Start()
    {
        base.Start();

        _locationText.text = _locationName;
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out ShipData shipData)) { return; }

        _playableDirector.Play();

        shipData.SetLocation(_locationName);
    }
}
