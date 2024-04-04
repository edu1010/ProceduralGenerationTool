
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TilesConPathFinding;
[CustomEditor(typeof(RandomRoomPlacementManager))]
public class RandomCreationOfRoomsEditor : Editor
    {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RandomRoomPlacementManager randomCreation = (RandomRoomPlacementManager)target;

        if (GUILayout.Button("Create rooms and conections"))
        {
            randomCreation.CreateNecesaryRooms();
        } 
        if (GUILayout.Button("Connect rooms"))
        {
            randomCreation.ConnectRoom();
        }
    }
}
#endif
