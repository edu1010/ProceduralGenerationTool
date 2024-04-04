#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using PlanetGenerator;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using Unity.VisualScripting.Dependencies.NCalc;
public class PlanetGeneratorEditorWindow : EditorWindow
{
    public float radius = 2f;
    public int subdivisions = 16;
    public float noise = 2f;
    private GameObject _planet;
    private GameObject _temp;
    private SphereGenerator _sphereGenerator;
    DataPlanet _data; 

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private GameObject gameObjectSeleccionado;
    private void OnEnable()
    {
        _data = Resources.Load<DataPlanet>("PlanetTest");
    }

    [MenuItem("Tool/Deprecreted/PlanetGenerator")]
    public static void ShowWindow()
    {
        GetWindow<PlanetGeneratorEditorWindow>("Planet Generator");
        
    }

    private void OnGUI()
    {
     
        GUILayout.Space(10);
        gameObjectSeleccionado = (GameObject)EditorGUILayout.ObjectField("GameObject", gameObjectSeleccionado, typeof(GameObject), true);
        if (GUILayout.Button("Create planet"))
        {
            _planet= new GameObject("Planet");
            //_planet = Instantiate(_planet);
            meshRenderer= _planet.AddComponent<MeshRenderer>();
            //meshRenderer.material = new Material("Unlit");
            meshFilter =  _planet.AddComponent<MeshFilter>();
            

            _temp= new GameObject("temp");
            _sphereGenerator= _temp.AddComponent<SphereGenerator>();
            
            meshFilter.mesh = _sphereGenerator.CreateSphereMesh(radius,subdivisions);
            //_planet = Instantiate(_planet);
            
            DestroyImmediate(_temp);

        } 
        GUILayout.Space(10);

        GUILayout.Label("radius: " + radius.ToString("F2"));
        radius = GUILayout.HorizontalSlider(radius, 0.0f, 100.0f);
        
        GUILayout.Space(10);

        GUILayout.Label("subdivisions: " + subdivisions.ToString("F2"));
        subdivisions = (int)GUILayout.HorizontalSlider(subdivisions, 0, 100);
        GUILayout.Space(10);
        GUILayout.Label("noise: " + noise.ToString("F2"));

        noise = GUILayout.HorizontalSlider(noise, 0, 100);
        GUILayout.Space(10);

        if (GUILayout.Button("Apply Noise"))
        {
            ApplyNoise();

        }

        GUILayout.Space(20);
        if (GUILayout.Button("Destroy planet"))
        {
            if(_planet!=null)
                DestroyImmediate(_planet);
            if(_temp!=null)
                DestroyImmediate(_temp);
        } 
    }
    private ComputeBuffer verticesBuffer;
    private ComputeBuffer heightsBuffer;
    private int kernelID;

    public void ApplyNoise()
    {
            _data._testValue=noise;

        Vector3[] vertices = meshFilter.mesh.vertices;
        verticesBuffer = new ComputeBuffer(vertices.Length, 12);
        verticesBuffer.SetData(vertices);
        
        heightsBuffer = new ComputeBuffer(vertices.Length, 4);
        kernelID = _data.planetShader.FindKernel("CSMain");
        
        
        // Asignar los Compute Buffers al Compute Shader
        _data.planetShader.SetBuffer(kernelID, "vertices", verticesBuffer);
        _data.planetShader.SetBuffer(kernelID, "heights", heightsBuffer);

        _data.planetShader.SetInt("numVertices", vertices.Length);
        
        // Asignar valores de prueba al Compute Shader
        _data.planetShader.SetFloat("testValue", noise);
        
        
        
        // Ejecutar el Compute Shader
        uint numVertices = (uint)vertices.Length;
        _data.planetShader.Dispatch(kernelID, (int)(numVertices / 512) + 1, 1, 1);
       //_data.planetShader.Dispatch(kernelID, (int)numVertices / 512, 1, 1);

        // Recuperar los datos del Compute Buffer de alturas generados por el Compute Shader
        //float[] heights = new float[vertices.Length];

        //Array newHeights = new Array[vertices.Length];

        //heightsBuffer.GetData(heights);
        AsyncGPUReadbackRequest request = AsyncGPUReadback.Request(heightsBuffer);
        AsyncGPUReadback.WaitAllRequests();

        float[] heights = request.GetData<float>().ToArray();
        Debug.Log("Hola he terminado: " + request.done);
       
        // Asignar las alturas a los vértices de la malla
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = heights[i];

            Debug.Log(heights[i]);
        }
        meshFilter.mesh.vertices = vertices;

        // Actualizar la malla
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();
        
        // Liberar los Compute Buffers
        verticesBuffer.Release();
        heightsBuffer.Release();
    }
    async void  CalculateHeight(AsyncGPUReadbackRequest request)
    {
        while (!request.done)
        {
            //wait
        }

    }

   

}
#endif
