using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleInEditorOnly : MonoBehaviour
{
    [SerializeField] private bool _disableGameobject = true;

    private void Awake()
    {
        if (_disableGameobject) { gameObject.SetActive(false); return; }

        GetComponent<Renderer>().enabled = false;
    }
}
