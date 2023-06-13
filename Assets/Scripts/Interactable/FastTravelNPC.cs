using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastTravelNPC : NPC
{
    #region Editor Fields

    [SerializeField] private FastTravelLocation[] _travelLocations;

    [SerializeField] private GameObject _fastTravelOptions;

    #endregion

    #region Public Properties

    public FastTravelLocation CurrentFastTravelLocation => _currentFastTravelLocation;

    #endregion

    #region Private Variables

    private FastTravelLocation _currentFastTravelLocation;

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
        _currentFastTravelLocation = _travelLocations[posIndex];
        shipFastTravel.WantToTravel = true;
        CurrentPlayer.GetComponent<PlayerInteractionController>().CheckExitInteraction();
    }

    #region Helper Classes

    [System.Serializable]
    public class FastTravelLocation
    {
        public int SceneIndex = -1;
        public Vector3 TravelPosition;
    }

    #endregion
}