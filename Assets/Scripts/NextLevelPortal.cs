using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class NextLevelPortal : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private Item _portalKey;
    [SerializeField] private ItemQuantity _portalKeyQuantity;
    [SerializeField] private PlayableDirector _entranceAnimation;


    #endregion

    #region Private Variable

    private Booster _booster;

    #endregion

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Booster booster)) { return; }

        if (MainInventory.Instance.InventoryDictionary.TryGetValue(_portalKey, out _portalKeyQuantity))

        _booster = booster;

        PortalPhase1();
    }

    private void PortalPhase1()
    {
        _booster.SetIsHovering(true);

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
        Play(_entranceAnimation);

        yield return new WaitForSeconds(30f);

        StartCoroutine(PortalPhase4());
    }

    private IEnumerator PortalPhase4()
    {
        Play(_portalAnimation);

        yield return new WaitForSeconds(1f);


    }
    //1 esta cerrado en los estados siguientes la nave no se debe mover hasta el final, quedandose en el lugar flotando
    //2 el jugador interactua con la puerta y esta vibra
    //3 la puerta empieza a hacer algo y vienen enemigos por 30 segundos
    //4 la puerta se abre
}