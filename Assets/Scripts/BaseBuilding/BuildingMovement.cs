using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMovement : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private BasePartType _basePartType;

    #endregion

    #region Unity Loops

    private void Start()
    {
        BasePartsManager.Instance.OnLocationChanged += HandleLocationChange;
    }

    private void OnDestroy()
    {
        BasePartsManager.Instance.OnLocationChanged -= HandleLocationChange;
    }

    #endregion

    private void HandleLocationChange()
    {
        if (!BasePartsManager.Instance.HasNextLocation()) { return; }

        Debug.Log("BasePartsManager has next location");

        StartCoroutine(TransformToBasePart());
    }

    private IEnumerator TransformToBasePart()
    {
        Debug.Log("TransformToBasePart");

        GameAudioManager.Instance.PlaySound(GameAudioManager.Instance.TransformToBasePart, transform.position);

        ParticleSystem buildingSmokeParticle = Instantiate(GameAssetsManager.Instance.BuildingSmokeParticle, transform).GetComponent<ParticleSystem>();
        buildingSmokeParticle.Play();
        buildingSmokeParticle.transform.SetParent(null);

        yield return new WaitForSeconds(3f);

        BasePart basePart = Instantiate(GameAssetsManager.Instance.BasePart).GetComponent<BasePart>();
        basePart.Initialize(gameObject, _basePartType);
    }
}
