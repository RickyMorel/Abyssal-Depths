using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class NextLevelTriggerZone : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private ItemQuantity _portalKeyNeededQuantity;
    [SerializeField] private PlayableDirector _entranceAnimation;
    [SerializeField] private ParticleSystem _portalParticle;
    [SerializeField] private ParticleSystem _rockDustParticle;

    #endregion

    #region Private Variables

    private bool _updateCheck = false;
    private bool _isInteracting = false;
    private PlayerInputHandler _playerInput;
    private PlayerCarryController _playerCarryController;

    #endregion

    #region Unity Loops

    private void Update()
    {
        if (!_updateCheck) { return; }

        if (Ship.Instance.ShipLandingController.Booster.CurrentPlayer != null) { Ship.Instance.ShipLandingController.Booster.CurrentPlayer.CheckExitInteraction(); }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Ship _)) { return; }

        if (!MainInventory.Instance.InventoryDictionary.TryGetValue(_portalKeyNeededQuantity.Item, out ItemQuantity inventoryPortalKey)) { return; }

        if (Ship.Instance.ShipLandingController.Booster.CurrentPlayer == null) { return; }
        
        if (_playerInput == null) { _playerInput = Ship.Instance.ShipLandingController.Booster.CurrentPlayer.GetComponent<PlayerInputHandler>(); }

        if (_playerCarryController == null) { _playerCarryController = _playerInput.GetComponent<PlayerCarryController>(); }
        
        if (_playerCarryController.CurrentSingleItem != null) { return; }

        if (!_playerInput.IsShooting_2) { return; }
        
        if (_isInteracting) { return; }

        _isInteracting = true;

        PortalPhase1(inventoryPortalKey);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Shield _)) { return; }

        _isInteracting = false;
    }

    #endregion

    private void PortalPhase1(ItemQuantity inventoryPortalKey)
    {
        if (!_isInteracting) { return; }

        Ship.Instance.ShipLandingController.Booster.LockHovering = true;
        Ship.Instance.ShipLandingController.Booster.SetIsHovering(true);
        MainInventory.Instance.RemoveItem(inventoryPortalKey);
        _updateCheck = true;

        PortalPhase2();
    }

    private void PortalPhase2()
    {
        //todo: minigame or something idk
        StartCoroutine(PortalPhase3());
    }

    private IEnumerator PortalPhase3()
    {
        //_entranceAnimation.Play();

        //Ship.Instance.GetComponentInChildren<ShipCamera>().ShakeCamera(5, 5, 30);

        _rockDustParticle.Play();

        yield return new WaitForSeconds(30f);

        _rockDustParticle.Stop();

        PortalPhase4();
    }

    private void PortalPhase4()
    {
        _portalParticle.Play();
        _updateCheck = false;
    }
    //1 esta cerrado en los estados siguientes la nave no se debe mover hasta el final, quedandose en el lugar flotando
    //2 el jugador interactua con la puerta y esta vibra
    //3 la puerta empieza a hacer algo y vienen enemigos por 30 segundos
    //4 la puerta se abre
}