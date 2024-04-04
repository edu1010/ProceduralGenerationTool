using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
//using static LevelData; 
[System.Serializable]
public struct GridSize
{
    public int maxHeight;
    public int maxWidth;
    public int minHeight;
    public int minWidth;

    public GridSize(int maxHeight, int maxWidth, int minHeight, int minWidth)
    {
        this.maxHeight = maxHeight;
        this.maxWidth = maxWidth;
        this.minHeight = minHeight;
        this.minWidth = minWidth;
    }
}
[System.Serializable]
public class Level
{
    public LevelData levelData;

#if UNITY_EDITOR
    public SerializedObject serializedLevelData;
    public SerializedProperty roomListProperty;
#endif
    [HideInInspector] public Vector2 scrollPosition;
    [HideInInspector] public bool[] roomFoldouts;
    [HideInInspector] public bool[][] eventFoldouts;
    
}


[System.Serializable]
public class LevelData :ScriptableObject
{
    [HideInInspector] public GridGenerator gridGenerator;
    [SerializeField]public List<RoomData> numberOfRooms = new List<RoomData>();
    [HideInInspector][SerializeField]public TypeOfCreation typeOfLevel;
}
public enum TypeOfCreation
{
    RoomsConnetedWithAStar,
    RogueLite,
    Automata
}
public enum TypeOfEditor
{
    MANUAL_EDITOR,
    AUTOLEVEL
}
[System.Serializable]
public class RoomData
{
    public int NumberOfEnemies;
    public int NumberOfSpwner;
    public List<EventData> EventsInTheRoom = new List<EventData>();

}
[System.Serializable]
public class EventData
{
    public float ProbabilityOfConversionFloorInEvent;
    public GameObject FloorEvent;
    public RoomEventController roomEventController;
}
