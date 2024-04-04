#if UNITY_EDITOR
using UnityEditor;

namespace TilesConPathFinding.CustomEditors
{
    [CustomEditor(typeof(ModifyScene))]
    public class ModifyScene :  Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This script adds a custom button in the Scene View window.", MessageType.Info);
        }
    }
}
#endif