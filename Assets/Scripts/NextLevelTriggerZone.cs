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

    #region Private Variable

    private Booster _booster;

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Shield _)) { return; }

        if (MainInventory.Instance.InventoryDictionary.TryGetValue(_portalKeyQuantity.Item, out _portalKeyQuantity))

        PortalPhase1();
    }

    private void PortalPhase1()
    {
        Ship.Instance.ShipLandingController.Booster.SetIsHovering(true);

        MainInventory.Instance.RemoveItem(_portalKeyQuantity);

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

        StartCoroutine(PortalPhase4());
    }

    private IEnumerator PortalPhase4()
    {
        _portalParticle.Play();

        yield return new WaitForSeconds(1f);
    }
    //1 esta cerrado en los estados siguientes la nave no se debe mover hasta el final, quedandose en el lugar flotando
    //2 el jugador interactua con la puerta y esta vibra
    //3 la puerta empieza a hacer algo y vienen enemigos por 30 segundos
    //4 la puerta se abre
}