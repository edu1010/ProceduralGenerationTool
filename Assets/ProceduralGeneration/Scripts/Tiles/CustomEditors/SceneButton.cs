#if UNITY_EDITOR    
using UnityEditor;
using UnityEngine;

namespace TilesConPathFinding.CustomEditors
{
    [ExecuteInEditMode]
    public class SceneButton : MonoBehaviour
    {
        public static int width=4; 
        public static int height=4;
        public static GridGenerator gridGenerator;
        public static RandomRoomPlacementManager randomRoomPlacementManager;
        static SceneButton()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            Handles.BeginGUI();
            GUILayout.BeginVertical();
            if (GUILayout.Button("Generate Room"))
            {
                Debug.Log("Room generator called");
                RoomGenerator.Instance.GenerateSelectedRoom(gridGenerator,width,height);
            }
            if (GUILayout.Button("Connect Room")) 
            {
                randomRoomPlacementManager.ConnectRoom();
            }

            GUILayout.BeginHorizontal();
            width = (int)EditorGUILayout.IntField("Width:", width);
            height = (int)EditorGUILayout.IntField("Height:", height);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            Handles.EndGUI();
        }
    }
}
#endif