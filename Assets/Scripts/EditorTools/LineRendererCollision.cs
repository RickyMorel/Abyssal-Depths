using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererCollision : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private MeshCollider _meshCollider;

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _meshCollider = GetComponent<MeshCollider>();
    }

    private void Update()
    {
        GenerateMeshCollider();
    }

    public void GenerateMeshCollider()
    {
        Mesh mesh = new Mesh();
        _lineRenderer.BakeMesh(mesh, true);
        _meshCollider.sharedMesh = mesh;
    }
}
