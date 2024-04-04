using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateSphere : MonoBehaviour
{
    public int divisions = 16;
    public float radius = 1.0f;

    private Mesh mesh;

    void Start() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        SphereGenerator ();
    }

    void SphereGenerator () {
        Vector3[] vertices = new Vector3[(divisions + 1) * (divisions + 1)];
        int[] triangles = new int[divisions * divisions * 6];

        int vertIndex = 0;
        for (int j = 0; j <= divisions; j++) {
            for (int i = 0; i <= divisions; i++) {
                Vector3 origin = transform.position;
                Vector3 right = transform.right;
                Vector3 up = transform.up;

                Vector3 facePoint = origin + 2.0f * radius * (right * i + up * j) / divisions;
                vertices[vertIndex] = facePoint.normalized * radius;
                vertIndex++;

                if (i < divisions && j < divisions) {
                    int topLeft = (j * (divisions + 1)) + i;
                    int topRight = topLeft + 1;
                    int bottomLeft = ((j + 1) * (divisions + 1)) + i;
                    int bottomRight = bottomLeft + 1;

                    triangles[vertIndex - 2] = topLeft;
                    triangles[vertIndex - 1] = bottomLeft;
                    triangles[vertIndex] = topRight;

                    triangles[vertIndex + 1] = topRight;
                    triangles[vertIndex + 2] = bottomLeft;
                    triangles[vertIndex + 3] = bottomRight;
                    vertIndex += 4;
                }
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
