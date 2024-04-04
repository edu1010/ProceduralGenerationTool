#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TilesConPathFinding.CustomEditors;

public class CustomEditor : EditorWindow
{
    GameData gd;
    private List<Level> levels = new List<Level>();
    private int currentLevelIndex = 0;
    public GameObject floor;
    int horizontalSizeOfTheGrid=100;
    int verticalSizeeOfTheGrid=50;
    DataNeededByMenu infoMenu;
    public TypeOfCreation typeOfCreation;
    public TypeOfEditor   typeOfEditor = TypeOfEditor.AUTOLEVEL;
    private RoomGenerator _roomGenerator;
    AnimationCurve GameDifficult = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));



    int automataWidth = 40;
    int automataHeight = 40;
    int automatanSteps = 1;
    float fillRate = 0.545f;


    DungeonGenerator _dungeonGenerator;

    int maxHeight=10;
    int maxWidth = 10   ;
    int minHeight =4;
    int minWidth=4;

    [MenuItem("Tool/Procedural level creator")]
    private static void OpenWindow()
    {
        CustomEditor window = GetWindow<CustomEditor>();
        window.titleContent = new GUIContent("Procedural level creator");
        window.Show();
    }

    private void OnEnable()
    {
        infoMenu = Resources.Load<DataNeededByMenu>("infoMenu");
        if(floor== null)
        {
            floor = infoMenu.FloorOfTheGrid;
        }
        CreateNewLevel();
        
    }

    private void CreateNewLevel()
    {
        Level newLevel = new Level();
        newLevel.levelData = ScriptableObject.CreateInstance<LevelData>();
        newLevel.serializedLevelData = new SerializedObject(newLevel.levelData);
        newLevel.roomListProperty = newLevel.serializedLevelData.FindProperty("numberOfRooms");
        
        newLevel.levelData.gridGenerator = new GridGenerator();
        newLevel.levelData.gridGenerator.floor= floor;
        newLevel.levelData.gridGenerator.horizontalSize= horizontalSizeOfTheGrid;
        newLevel.levelData.gridGenerator.verticalSize= verticalSizeeOfTheGrid;
        

        newLevel.roomFoldouts = new bool[newLevel.roomListProperty.arraySize];
        newLevel.eventFoldouts = new bool[newLevel.roomListProperty.arraySize][];
        for (int i = 0; i < newLevel.roomListProperty.arraySize; i++)
        {
            newLevel.eventFoldouts[i] = new bool[newLevel.levelData.numberOfRooms[i].EventsInTheRoom.Count];
        }
        

        levels.Add(newLevel);
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label("Level: ", GUILayout.Width(60));
        currentLevelIndex = EditorGUILayout.Popup(currentLevelIndex, GetLevelNames(), GUILayout.Width(120));
        GUILayout.EndHorizontal();
        

        floor = (GameObject)EditorGUILayout.ObjectField("Floor of the grid", floor, typeof(GameObject), true);
        GUILayout.Space(10);

        typeOfCreation = (TypeOfCreation)EditorGUILayout.EnumPopup(name, typeOfCreation);
        typeOfEditor = (TypeOfEditor)EditorGUILayout.EnumPopup(name, typeOfEditor);
        if (typeOfCreation==TypeOfCreation.RoomsConnetedWithAStar)
        {
            
            horizontalSizeOfTheGrid = (int)EditorGUILayout.IntField("Horizontal size of the grid:", horizontalSizeOfTheGrid);
            verticalSizeeOfTheGrid = (int)EditorGUILayout.IntField("Vertical size of the grid:", verticalSizeeOfTheGrid);
            if(typeOfEditor != TypeOfEditor.MANUAL_EDITOR)
            {
                GUILayout.Label("Room info: ", GUILayout.Width(100));
                GUILayout.BeginHorizontal();
                maxHeight = (int)EditorGUILayout.IntField("Max height:", maxHeight);
                maxWidth = (int)EditorGUILayout.IntField("Max width:", maxWidth);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                minHeight = (int)EditorGUILayout.IntField("Min height:", minHeight);
                minWidth = (int)EditorGUILayout.IntField("Min width:", minWidth);
                GUILayout.EndHorizontal();
            }
            
        }
        if(typeOfCreation == TypeOfCreation.RogueLite)
        {
            _dungeonGenerator = new DungeonGenerator();
        }

        if (typeOfEditor != TypeOfEditor.MANUAL_EDITOR)
        {
            GameDifficult = (AnimationCurve)EditorGUILayout.CurveField("Game Difficult curve:", GameDifficult);
            if (GUILayout.Button("Add Level"))
            {
                CreateNewLevel();
            }
        }
        GUILayout.Space(10);

        if (typeOfCreation == TypeOfCreation.Automata) 
        {
            automataWidth = (int)EditorGUILayout.IntField("Horizontal size of the grid:", automataWidth);
            automataHeight = (int)EditorGUILayout.IntField("´Vertical size of the grid:", automataHeight);
            automatanSteps = (int)EditorGUILayout.IntField("´Number of steps:", automatanSteps);
            fillRate = (float)EditorGUILayout.FloatField("´Fill rate:", fillRate);
            
            //GenerateLevel();
            //return; 
        }

        if (currentLevelIndex >= 0 && currentLevelIndex < levels.Count)
        {
            Level currentLevel = levels[currentLevelIndex];
            EditorGUI.BeginChangeCheck();
            currentLevel.serializedLevelData.Update();
            DrawLevelInterface(currentLevel);
            currentLevel.serializedLevelData.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(currentLevel.levelData);
            }
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Create GameData Only"))
        {
            CreateGameData();
            /*
            GameObject GameData = new GameObject("GameData");
            GameData gd = GameData.AddComponent<GameData>();
            gd.levels = levels;*/
            
        }
    }

    private void DrawLevelInterface(Level level)
    {
        GUILayout.Label("Level Data", EditorStyles.boldLabel);

        GUILayout.Space(10);
        GenerateLevel();
       
        GUILayout.Space(10);
        if (typeOfEditor == TypeOfEditor.MANUAL_EDITOR) { return; }
        SerializedProperty roomListProperty = level.roomListProperty;

        EditorGUILayout.BeginHorizontal();
        if(typeOfCreation== TypeOfCreation.Automata)
        {
            GUILayout.Label("Events in the map "+ roomListProperty.arraySize);
            if (roomListProperty.arraySize < 1)
            {
                roomListProperty.arraySize++;
                level.serializedLevelData.ApplyModifiedProperties();
                level.roomFoldouts = new bool[roomListProperty.arraySize];
                level.eventFoldouts = new bool[roomListProperty.arraySize][];
            }
            if (GUILayout.Button("Add Room"))
            {
                //roomListProperty.arraySize++;
                //level.serializedLevelData.ApplyModifiedProperties();
                //level.roomFoldouts = new bool[roomListProperty.arraySize];
                //level.eventFoldouts = new bool[roomListProperty.arraySize][];
            }
        }
        else
        {
            GUILayout.Label("Number of Rooms: " + roomListProperty.arraySize, EditorStyles.boldLabel);

            if (GUILayout.Button("Add Room"))
            {
                roomListProperty.arraySize++;
                level.serializedLevelData.ApplyModifiedProperties();
                level.roomFoldouts = new bool[roomListProperty.arraySize];
                level.eventFoldouts = new bool[roomListProperty.arraySize][];
            }

        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        level.scrollPosition = EditorGUILayout.BeginScrollView(level.scrollPosition);

        int roomToRemoveIndex = -1;
        int eventToRemoveIndex = -1;
        int roomOfEventToRemoveIndex = -1;

        for (int i = 0; i < roomListProperty.arraySize; i++)
        {
            SerializedProperty roomProperty = roomListProperty.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginVertical(GUI.skin.box);

            if (typeOfCreation == TypeOfCreation.Automata)
            {
                level.roomFoldouts[i] = EditorGUILayout.Foldout(level.roomFoldouts[i], "Map " + (i + 1), true);
            }
            else
            {
                level.roomFoldouts[i] = EditorGUILayout.Foldout(level.roomFoldouts[i], "Room " + (i + 1), true);
            }

            if (level.roomFoldouts[i])
            {
                EditorGUILayout.PropertyField(roomProperty.FindPropertyRelative("NumberOfEnemies"));
                EditorGUILayout.PropertyField(roomProperty.FindPropertyRelative("NumberOfSpwner"));

                GUILayout.Space(5);

                SerializedProperty eventsListProperty = roomProperty.FindPropertyRelative("EventsInTheRoom");

                GUILayout.Label("Events in the Room", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Number of Events: " + eventsListProperty.arraySize, EditorStyles.boldLabel);

                if (GUILayout.Button("Add Event"))
                {
                    eventsListProperty.arraySize++;
                    level.serializedLevelData.ApplyModifiedProperties();
                    level.eventFoldouts[i] = new bool[eventsListProperty.arraySize];
                }
                else
                {
                    
                    if(level.eventFoldouts[i]==null)
                    {
                        int numberOfEnemies = roomProperty.FindPropertyRelative("NumberOfEnemies").intValue;
                        if (numberOfEnemies > 0)
                        {
                            eventsListProperty.arraySize++;
                            level.serializedLevelData.ApplyModifiedProperties();
                            level.eventFoldouts[i] = new bool[eventsListProperty.arraySize];
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();

                for (int j = 0; j < eventsListProperty.arraySize; j++)
                {
                    SerializedProperty eventProperty = eventsListProperty.GetArrayElementAtIndex(j);

                    EditorGUILayout.BeginVertical(GUI.skin.box);

                    level.eventFoldouts[i][j] = EditorGUILayout.Foldout(level.eventFoldouts[i][j], "Event " + (j + 1), true);

                    if (level.eventFoldouts[i][j])
                    {
                        EditorGUILayout.PropertyField(eventProperty.FindPropertyRelative("ProbabilityOfConversionFloorInEvent"));
                        EditorGUILayout.PropertyField(eventProperty.FindPropertyRelative("FloorEvent"));

                        if (GUILayout.Button("Remove Event"))
                        {
                            eventToRemoveIndex = j;
                            roomOfEventToRemoveIndex = i;
                        }
                    }

                    EditorGUILayout.EndVertical();
                }

                if (GUILayout.Button("Remove Room"))
                {
                    roomToRemoveIndex = i;
                }
            }

            EditorGUILayout.EndVertical();
        }
        
        EditorGUILayout.EndScrollView();

        if (roomToRemoveIndex != -1)
        {
            roomListProperty.DeleteArrayElementAtIndex(roomToRemoveIndex);
            level.serializedLevelData.ApplyModifiedProperties();
            level.roomFoldouts = new bool[roomListProperty.arraySize];
            level.eventFoldouts = new bool[roomListProperty.arraySize][];
        }

        if (eventToRemoveIndex != -1)
        {
            SerializedProperty roomProperty = roomListProperty.GetArrayElementAtIndex(roomOfEventToRemoveIndex);
            SerializedProperty eventsListProperty = roomProperty.FindPropertyRelative("EventsInTheRoom");
            eventsListProperty.DeleteArrayElementAtIndex(eventToRemoveIndex);
            level.serializedLevelData.ApplyModifiedProperties();
            level.eventFoldouts[roomOfEventToRemoveIndex] = new bool[eventsListProperty.arraySize];
        }

        level.serializedLevelData.ApplyModifiedProperties();
    }

    private string[] GetLevelNames()
    {
        string[] levelNames = new string[levels.Count];
        for (int i = 0; i < levels.Count; i++)
        {
            levelNames[i] = "Level " + (i + 1);
        }
        return levelNames;
    }
    public void GenerateLevel()
    {
        if (GUILayout.Button("Generate level"))
        {
            CreateGameData();

            if (typeOfCreation == TypeOfCreation.RoomsConnetedWithAStar)
            {
                if (_roomGenerator == null || _roomGenerator.gameObject == null)
                {
                    GameObject roomGenerator = new("RoomGenerator");
                    _roomGenerator = roomGenerator.AddComponent<RoomGenerator>();
                    _roomGenerator.CreatePrefab = infoMenu.NodeInfoToCollapse;
                    gd.roomGenerator = _roomGenerator;

                }
                if (typeOfEditor == TypeOfEditor.MANUAL_EDITOR)
                {

                    levels[currentLevelIndex].levelData.gridGenerator.manualEditor = true;

                    levels[currentLevelIndex].levelData.gridGenerator.horizontalSize = horizontalSizeOfTheGrid;
                    levels[currentLevelIndex].levelData.gridGenerator.verticalSize = verticalSizeeOfTheGrid;

                    levels[currentLevelIndex].levelData.gridGenerator.GenerateGrid3D(floor);


                    SceneButton sceneButton = new SceneButton();
                    SceneButton.gridGenerator = levels[currentLevelIndex].levelData.gridGenerator;

                    GameObject gb = new GameObject("RandomPlaceRoomManager");
                    RandomRoomPlacementManager randomRoomPlacementManager = gb.AddComponent<RandomRoomPlacementManager>();
                    randomRoomPlacementManager.Init(maxHeight, maxWidth, levels[currentLevelIndex].levelData.gridGenerator, minHeight, minWidth, levels[currentLevelIndex].roomListProperty.arraySize, infoMenu.NodeInfoToCollapse, infoMenu.Portal);
                    SceneButton.randomRoomPlacementManager = randomRoomPlacementManager;
                }
                else
                {
                    gd.roomGenerator = _roomGenerator;
                    gd.GenerateNextLevel(currentLevelIndex);
                    /*level.levelData.gridGenerator.GenerateGrid3D(floor);
                    GameObject gb = new GameObject("RandomPlaceRoomManager");
                    RandomRoomPlacementManager randomRoomPlacementManager = gb.AddComponent<RandomRoomPlacementManager>();
                    randomRoomPlacementManager.Init(maxHeight, maxWidth, level.levelData.gridGenerator, minHeight, minWidth, level.roomListProperty.arraySize, infoMenu.NodeInfoToCollapse, infoMenu.Portal);
                    randomRoomPlacementManager.CreateNecesaryRooms();*/

                }

            }
            if (typeOfCreation == TypeOfCreation.RogueLite)
            {

                gd.GenerateNextLevel(currentLevelIndex);
                // _dungeonGenerator.CreateDungeon(level.levelData.numberOfRooms.Count);
            }
            if (typeOfCreation == TypeOfCreation.Automata)
            {

                gd.GenerateNextLevel(currentLevelIndex);
                // _dungeonGenerator.CreateDungeon(level.levelData.numberOfRooms.Count);
            }
            gd.GenerateEvents();

        }
    }
    public void CreateGameData()
    {
        if(gd==null)
        {
            GameObject GameData = new GameObject("GameData");
            gd = GameData.AddComponent<GameData>();
        }
        
        gd.roomGenerator = _roomGenerator;
        gd.dungeonGenerator= _dungeonGenerator;
        gd.levels = levels;
        gd.typeOfCreation = typeOfCreation;
        gd.typeOfEditor = typeOfEditor;
        gd.floor = floor;
        gd.gridSize = new GridSize(maxHeight, maxWidth, minHeight, minWidth);
        gd.infoMenu = infoMenu;
        gd.GameDifficult = GameDifficult;
        gd.NumberOfLevels= levels.Count;

        gd.automataWidth = automataWidth;
        gd.automataHeight = automataHeight;
        gd.automatanSteps = automatanSteps;
        gd.fillRate = fillRate;


    }


}
#endif