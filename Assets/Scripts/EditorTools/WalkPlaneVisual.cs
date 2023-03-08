using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is just a class find the walk plane instance
public class WalkPlaneVisual : MonoBehaviour
{
    #region Private Variables

    private static WalkPlaneVisual _instance;

    #endregion

    #region Public Properties

    public static WalkPlaneVisual Instance { get { return _instance; } }

    #endregion

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public bool IsWithinBounds(Vector3 pos)
    {
        if (pos.z > transform.position.z + transform.localScale.z / 2) { return false; }
        if (pos.z < transform.position.z - transform.localScale.z / 2) { return false; }

        return true;
    }
}
