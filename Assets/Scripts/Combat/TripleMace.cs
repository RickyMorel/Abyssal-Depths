using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleMace : MeleeWeapon
{
    #region Editor Fields

    [SerializeField] private GameObject[] _maceHead;

    #endregion

    #region Unity Loops

    public override void Awake()
    {
        _rb = _attackHitBox[0].GetComponent<Rigidbody>();
    }

    public override void OnDestroy()
    {
        //do nothing
    }

    public override void OnEnable()
    {
        for (int i = 0; i < _maceHead.Length; i++)
        {
            _maceHead[i].transform.parent = null;
        }
    }

    public override void OnDisable()
    {
        if (_parentTransform == null) { return; }

        for (int i = 0; i < _maceHead.Length; i++)
        {
            _maceHead[i].transform.parent = _parentTransform;
        }
    }

    #endregion

    public override void HandleHitParticles(GameObject obj)
    {
        //do nothing
    }
}