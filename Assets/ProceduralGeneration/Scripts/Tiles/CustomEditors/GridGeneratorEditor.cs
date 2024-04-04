#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace TilesConPathFinding.CustomEditors
{
    [CustomEditor(typeof(GridGenerator))]
    public class GridGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            /*GridGenerator gridManager = (GridGenerator)target;

            if (GUILayout.Button("Create Grid"))
            {
                gridManager.GenerateGrid3D();
            } 
            if (GUILayout.Button("Destroy Grid"))
            {
                gridManager.DeleteGrid();
            }*/
        }
    }
}
#endif
