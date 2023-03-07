using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;

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
        Transform blockingPlane_2 = transform.Find("BlockingPlane_2");

        for (int i = 0; i < 2; i++)
        {
            if (i == 0 && blockingPlane_1 != null) { continue; }
            if (i == 1 && blockingPlane_2 != null) { continue; }

            bool isFirstPlane = i == 0;

            //Spawn and Scale Object
            GameObject newBlockingPlane = GameObject.CreatePrimitive(PrimitiveType.Cube);
            newBlockingPlane.name = isFirstPlane ? "BlockingPlane_1" : "BlockingPlane_2";
            newBlockingPlane.transform.parent = transform;
            newBlockingPlane.transform.localScale = new Vector3(1f, 500f, 100f);
            newBlockingPlane.isStatic = true;

            //Add Components
            newBlockingPlane.GetComponent<Renderer>().enabled = false;
            DestroyImmediate(newBlockingPlane.GetComponent<Collider>());
            NavMeshObstacle navMeshObstacle = newBlockingPlane.AddComponent<NavMeshObstacle>();
            navMeshObstacle.carving = true;

            //Position Object
            int positionMultiplier = isFirstPlane ? 1 : -1;
            Vector3 spawnPos = new Vector3(0f, 0f, (newBlockingPlane.transform.localScale.z / 2f + 0.5f) * positionMultiplier);
            newBlockingPlane.transform.localPosition = spawnPos;
        }
    }

    public bool IsWithinBounds(Vector3 pos)
    {
        if (pos.z > transform.position.z + transform.localScale.z / 2) { return false; }
        if (pos.z < transform.position.z - transform.localScale.z / 2) { return false; }

        return true;
    }
}
