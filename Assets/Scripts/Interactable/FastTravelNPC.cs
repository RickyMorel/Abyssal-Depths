using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastTravelNPC : NPC
{
    #region Editor Fields

    [SerializeField] private Transform[] _travelPos;

    [SerializeField] private GameObject _fastTravelOptions;

    #endregion

    #region Public Properties

    public Transform TravelToPosition => _travelToPosition;

    #endregion

    #region Private Variables

    private Transform _travelToPosition;

    #endregion

    #region Unity Loops

    public virtual void Update()
    {
        DisplayUI();
    }

    #endregion

    private void DisplayUI()
    {
        if (CurrentPlayer == null)
        {
            _fastTravelOptions.SetActive(false);
        }
        else
        {
            _fastTravelOptions.SetActive(true);
        }
    }

    public void TravelTo(int posIndex)
    {
        ShipFastTravel shipFastTravel = Ship.Instance.GetComponent<ShipFastTravel>();
        shipFastTravel.FastTravelNPC = this;
        _travelToPosition = _travelPos[posIndex];
        shipFastTravel.WantToTravel = true;
        CurrentPlayer.GetComponent<PlayerInteractionController>().CheckExitInteraction();
    }
}