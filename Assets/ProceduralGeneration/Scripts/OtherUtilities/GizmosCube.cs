using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosCube : MonoBehaviour
{
    [SerializeField] private float cubeSize = 10f;
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        //Gizmos.DrawCube(transform.position,size);
        Vector3 center = transform.position;
        float halfSize = cubeSize / 2f;

        Vector3 a = center + new Vector3(-halfSize, -halfSize, -halfSize);
        Vector3 b = center + new Vector3(-halfSize, -halfSize, halfSize);
        Vector3 c = center + new Vector3(-halfSize, halfSize, -halfSize);
        Vector3 d = center + new Vector3(-halfSize, halfSize, halfSize);
        Vector3 e = center + new Vector3(halfSize, -halfSize, -halfSize);
        Vector3 f = center + new Vector3(halfSize, -halfSize, halfSize);
        Vector3 g = center + new Vector3(halfSize, halfSize, -halfSize);
        Vector3 h = center + new Vector3(halfSize, halfSize, halfSize);

        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(a, c);
        Gizmos.DrawLine(a, e);
        Gizmos.DrawLine(b, d);
        Gizmos.DrawLine(b, f);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(c, g);
        Gizmos.DrawLine(d, h);
        Gizmos.DrawLine(e, f);
        Gizmos.DrawLine(e, g);
        Gizmos.DrawLine(f, h);
        Gizmos.DrawLine(g, h);
    }
}
