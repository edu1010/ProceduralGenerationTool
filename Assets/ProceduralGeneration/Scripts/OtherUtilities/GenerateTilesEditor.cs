#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[UnityEditor.CustomEditor(typeof(GenerateTiles))]
public class GenerateTilesEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        GenerateTiles generateTile = (GenerateTiles)target;
        if (GUILayout.Button("Generate Tile"))
        {
            generateTile.GenerateListOfTiles();
        }
        base.OnInspectorGUI();

        
        
    }
}
#endif