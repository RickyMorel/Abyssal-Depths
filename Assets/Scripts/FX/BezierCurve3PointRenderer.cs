//#define RENDER_LINES

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BezierCurve3PointRenderer : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private Transform _point1;
    [SerializeField] private Transform _point2;
    [SerializeField] private Transform _point3;
    [SerializeField] private int _vertexCount = 12;

    #endregion

    #region Private Variables

    private LineRenderer _lineRenderer;

    #endregion
    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        var pointList = new List<Vector3>();
        for (float ratio = 0f; ratio <= 1; ratio += 1.0f / _vertexCount)
        {
            var tangentLineVertex1 = Vector3.Lerp(_point1.position, _point2.position, ratio);
            var tangentLineVertex2 = Vector3.Lerp(_point2.position, _point3.position, ratio);
            var bezierPoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
            pointList.Add(bezierPoint);
        }
        _lineRenderer.positionCount = pointList.Count;
        _lineRenderer.SetPositions(pointList.ToArray());
    }

#if RENDER_LINES

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_point1.position, _point2.position);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_point2.position, _point3.position);

        Gizmos.color = Color.red;
        for (float ratio = 0.5f / _vertexCount; ratio < 1; ratio += 1.0f / _vertexCount)
        {
            Gizmos.DrawLine(Vector3.Lerp(_point1.position, _point2.position, ratio), Vector3.Lerp(_point2.position, _point3.position, ratio));
        }
    }

#endif
}
