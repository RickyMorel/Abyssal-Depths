using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelTriggerZone : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private BoxCollider _nextLevelTriggerZone;
    [SerializeField] private float _portalSuctionSpeed = 500;
    [SerializeField] private SceneLoader _sceneLoader;

    #endregion

    #region Unity Loops

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Shield _)) { return; }

        if (Ship.Instance.ShipLandingController.Booster.CanUse != false) { Ship.Instance.ShipLandingController.Booster.TurnOffEngine(); }

        if (Vector3.Distance(transform.position, other.transform.position) > 15) { Ship.Instance.AddForceToShip((transform.position - other.transform.position).normalized * _portalSuctionSpeed, ForceMode.Force); }
        else { StartCoroutine(NextLevel()); }
    }

    private IEnumerator NextLevel()
    {
        TimelinesManager.Instance.CameraFadeTimeline.Play();

        yield return new WaitForSeconds(1f);

        _sceneLoader.LoadScene(3);
    }

    #endregion
}