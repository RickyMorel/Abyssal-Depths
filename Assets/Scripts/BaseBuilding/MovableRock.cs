using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class MovableRock : MonoBehaviour
{
    #region EditorFields

    [SerializeField] private ParticleSystem _drillingParticles;
    [SerializeField] private GameObject _rockExplosionFxPrefab;
    [SerializeField] private GameObject _rockMesh;

    #endregion

    #region Private Variables

    private Drill _drill;
    private bool _wantToDestroyRock = false;
    private float _timer;

    #endregion

    #region Unity Loops

    private void Update()
    {
        if (_wantToDestroyRock) { _timer += Time.deltaTime; DestroyFx(); }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.GetComponent<Drill>()) { return; }

        Debug.Log("OnTriggerEnter: Drill");

        _drill = other.gameObject.GetComponent<Drill>();

        transform.SetParent(_drill.transform);
        _drillingParticles.Play();
        _drill.OnDestroyCurrentRock += DestroyThisRock;
    }

    #endregion

    private void DestroyThisRock()
    {
        _rockExplosionFxPrefab = Instantiate(_rockExplosionFxPrefab, _drill.transform.position, _drill.transform.rotation);

        _rockExplosionFxPrefab.transform.SetParent(null);

        _wantToDestroyRock = true;

        
    }

    private void DestroyFx()
    {
        _drillingParticles.Stop();

        _rockMesh.GetComponent<PlayableDirector>().Play();

        if (_timer >= 1) { _rockExplosionFxPrefab.GetComponent<ParticleSystem>().Play(); Destroy(gameObject); }
    }
}