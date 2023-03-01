using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is just a class find the walk plane instance
[ExecuteInEditMode]
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
        if (!Application.isPlaying) { return; }

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void OnEnable()
    {
        if (!Application.isEditor || Application.isPlaying) { return; }

        CreateNavMeshBlockerGOs();
    }

    private void CreateNavMeshBlockerGOs()
    {
        Transform blockingPlane_1 = transform.Find("BlockingPlane_1");

        if (blockingPlane_1 != null)
        {
            //blockingPlane_1 = blockingPlane_1.gameObject;
            //blockingPlane_1.transform.position = _enemyBot.Path[0].Position;
            return;
        }

        GameObject newBlockingPlane_1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        newBlockingPlane_1.name = "BlockingPlane_1";
        newBlockingPlane_1.transform.parent = transform;
        newBlockingPlane_1.transform.localScale = new Vector3(1f, 500f, 100f);
        newBlockingPlane_1.isStatic = true;
        newBlockingPlane_1.GetComponent<Renderer>().enabled = false;
        DestroyImmediate(newBlockingPlane_1.GetComponent<Collider>());

        Vector3 spawnPos = new Vector3(0f, 0f, newBlockingPlane_1.transform.localScale.z / 2f + 0.5f);

        newBlockingPlane_1.transform.localPosition = spawnPos;
    }

    public bool IsWithinBounds(Vector3 pos)
    {
        if (pos.z > transform.position.z + transform.localScale.z / 2) { return false; }
        if (pos.z < transform.position.z - transform.localScale.z / 2) { return false; }

        return true;
    }
}
