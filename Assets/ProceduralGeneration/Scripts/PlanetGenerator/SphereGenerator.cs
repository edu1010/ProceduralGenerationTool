using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetGenerator
{
    //Codigo basado en el metodo Normalized Cube que se puede encontrar aqui https://medium.com/@oscarsc/four-ways-to-create-a-mesh-for-a-sphere-d7956b825db4  N
    public class SphereGenerator : MonoBehaviour
    {
        
        public Mesh CreateSphereMesh(float radius, int subdivisions)
        {
            Mesh mesh = new Mesh();

            // Calculate vertices
            Vector3[] vertices = new Vector3[(subdivisions + 1) * (subdivisions + 1)];
            Vector2[] uvs = new Vector2[vertices.Length];
            int index = 0;
            float step = 1f / subdivisions;
            for (float j = 0; j <= 1f; j += step)
            {
                for (float i = 0; i <= 1f; i += step)
                {
                    Vector3 vertex = GetSphereVertex(i, j, subdivisions) * radius;
                    vertices[index] = vertex;
                    uvs[index] = new Vector2(i, j);
                    index++;
                }
            }

            mesh.vertices = vertices;
            mesh.uv = uvs;

            // Calculate triangles
            int[] triangles = new int[subdivisions * subdivisions * 6];
            index = 0;
            int vertexCount = subdivisions + 1;
            for (int j = 0; j < subdivisions; j++)
            {
                for (int i = 0; i < subdivisions; i++)
                {
                    int a = i + j * vertexCount;
                    int b = i + (j + 1) * vertexCount;
                    int c = (i + 1) + j * vertexCount;
                    int d = (i + 1) + (j + 1) * vertexCount;
                    triangles[index++] = a;
                    triangles[index++] = d;
                    triangles[index++] = c;
                    triangles[index++] = a;
                    triangles[index++] = b;
                    triangles[index++] = d;
                }
            }

            mesh.triangles = triangles;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        private Vector3 GetSphereVertex(float i, float j, int subdivisions)
        {
            float theta = i * 2f * Mathf.PI;
            float phi = j * Mathf.PI;
            Vector3 vertex = new Vector3(Mathf.Sin(phi) * Mathf.Cos(theta), Mathf.Cos(phi),
                Mathf.Sin(phi) * Mathf.Sin(theta));
            return vertex;
        }

    }
}

