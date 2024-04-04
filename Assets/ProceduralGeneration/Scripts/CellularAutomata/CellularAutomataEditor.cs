#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[UnityEditor.CustomEditor(typeof(CellularAutomata))]
[CanEditMultipleObjects]
public class CellularAutomataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CellularAutomata cellular = (CellularAutomata)target;
        DrawDefaultInspector();


        if (GUILayout.Button("Create automataGrid"))
        {
            cellular.InitAutomata();
        }
        if (GUILayout.Button("Destroy Grid"))
        {
            cellular.DestroyTilemap();
        }
    }
}

#endif
