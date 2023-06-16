using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class NextLevelTriggerZone : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private ItemQuantity _portalKeyQuantity;
    [SerializeField] private PlayableDirector _entranceAnimation;
    [SerializeField] private ParticleSystem _portalParticle;
    [SerializeField] private ParticleSystem _rockDustParticle;

    #endregion

    #region Private Variables

    private bool _updateCheck = false;
    private PlayerInputHandler _playerInput;

    #endregion

    private void Update()
    {
        if (!_updateCheck) { return; }

        if (Ship.Instance.ShipLandingController.Booster.CurrentPlayer != null) { Ship.Instance.ShipLandingController.Booster.CurrentPlayer.CheckExitInteraction(); }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Shield _)) { return; }

        if (!MainInventory.Instance.InventoryDictionary.TryGetValue(_portalKeyQuantity.Item, out _portalKeyQuantity)) { return; }

        if (Ship.Instance.ShipLandingController.Booster.CurrentPlayer == null) { return; }

        if (Ship.Instance.ShipLandingController.Booster.CurrentPlayer.GetComponent<PlayerInputHandler>()) PortalPhase1();
    }

    private void PortalPhase1()
    {
        Ship.Instance.ShipLandingController.Booster.LockHovering = true;
        Ship.Instance.ShipLandingController.Booster.SetIsHovering(true);
        MainInventory.Instance.RemoveItem(_portalKeyQuantity);
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

        Ship.Instance.GetComponentInChildren<ShipCamera>().ShakeCamera(5, 5, 30);

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