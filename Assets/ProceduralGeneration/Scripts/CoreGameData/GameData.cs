using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Scene = UnityEngine.SceneManagement.Scene;

[ExecuteAlways]
[System.Serializable]
public class GameData : MonoBehaviour
{
    public AnimationCurve GameDifficult;//length and keys act as multiplayer
    EnemiesData enemiesData;
    public int NumberOfLevels = 1;
    public int MaxNumberOfLevels => levels.Count;//Levels predefine by the player
    public List<Level> levels;
    public int indexCurrentLevel = 0;
    [SerializeField] public TypeOfEditor typeOfEditor;
    [SerializeField] public TypeOfCreation typeOfCreation;
    [SerializeField] public GridSize gridSize;
    public GameObject floor;
    public DataNeededByMenu infoMenu;
    public RoomGenerator roomGenerator;
    public DungeonGenerator dungeonGenerator;
    public CellularAutomata cellularAutomata;
    public CellularAutomataData cellularAutomataData;
    public static GameData Instance;
    public float radiusOFSpawn2d = 2f;
    int indexOfScene = 0;

    public int   automataWidth = 40;
    public int   automataHeight = 40;
    public int   automatanSteps = 1;
    public float fillRate = 0.545f;
    Dictionary<String, Scene> ScenesToGo=new();
    float dificultMultiplayer = 1;
    public List<GameObject> ObjectToMaintainActive=new();
    private void Awake()
    {
        if(Instance == null)
        {
            Instance= this;
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += MakeDontDestroy;

#else
            DontDestroyOnLoad(gameObject);
#endif
        }
        else
        {
            DestroyImmediate(Instance);
        }
    }
#if UNITY_EDITOR

    private void MakeDontDestroy(PlayModeStateChange editorState)
    {
        if(editorState == PlayModeStateChange.EnteredPlayMode || editorState == PlayModeStateChange.ExitingEditMode)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
#endif
    private void OnEnable()
    {
        enemiesData = Resources.Load<EnemiesData>("EnemiesData");
    }
    private void Start()
    {
        if(infoMenu== null)
            infoMenu = Resources.Load<DataNeededByMenu>("infoMenu");
    }
    //This only apply when the player has not define the level to crate
    public void ApplyDifficult()
    {
        if (GameDifficult.keys.Length < 1)
        {
            dificultMultiplayer = 1;
            return;
        }
        int keyToWatch = (MaxNumberOfLevels * indexCurrentLevel) / GameDifficult.keys.Length;
        if(keyToWatch > GameDifficult.keys.Length-1)
        {
            keyToWatch = GameDifficult.keys.Length-1;
        }
        dificultMultiplayer = GameDifficult.keys[keyToWatch].value - GameDifficult.keys[0].value;
    }

    public void GenerateNextLevel(int index)
    {
        indexCurrentLevel=index;
        GenerateNextLevel();
    }

    public void GenerateNextLevel()
    {

        if (typeOfCreation == TypeOfCreation.RoomsConnetedWithAStar)
        {
            if (roomGenerator == null || roomGenerator.gameObject == null)
            {
                GameObject roomGenerator = new("RoomGenerator");
                DontDestroyOnLoad(roomGenerator);
                ObjectToMaintainActive.Add(roomGenerator);
                this.roomGenerator = roomGenerator.AddComponent<RoomGenerator>();
                this.roomGenerator.CreatePrefab = infoMenu.NodeInfoToCollapse;
            }
            if(indexCurrentLevel>=MaxNumberOfLevels) 
            {
                indexCurrentLevel = 0;
            }
            levels[indexCurrentLevel].levelData.gridGenerator.GenerateGrid3D(floor);
            GameObject gb = new GameObject("RandomPlaceRoomManager");
            RandomRoomPlacementManager randomRoomPlacementManager = gb.AddComponent<RandomRoomPlacementManager>();
            randomRoomPlacementManager.Init
                (gridSize.maxHeight, gridSize.maxWidth,
                levels[indexCurrentLevel].levelData.gridGenerator,
                gridSize.minHeight, gridSize.minWidth,
                levels[indexCurrentLevel].roomFoldouts.Length,
                infoMenu.NodeInfoToCollapse, infoMenu.Portal);
            randomRoomPlacementManager.CreateNecesaryRooms();
            int i = 0;
            foreach (var room in levels[indexCurrentLevel].levelData.numberOfRooms)
            {
                if (room.EventsInTheRoom.Count < 0)
                {
                    room.EventsInTheRoom.Add(new());
                }
                foreach(var  eventRoom in room.EventsInTheRoom)
                {
                    if(i>= randomRoomPlacementManager.IndependentRooms.Count)
                    {
                        continue;
                    }
                    eventRoom.roomEventController = randomRoomPlacementManager.IndependentRooms[i].roomEventController;
                   // Debug.Log("patata"+randomRoomPlacementManager.IndependentRooms[i].roomEventController.gameObject.name);  
                }
                i++;
            }

        }
        if (typeOfCreation == TypeOfCreation.RogueLite)
        {
            if (indexCurrentLevel >= MaxNumberOfLevels)
            {
                ApplyDifficult();
                dungeonGenerator.CreateDungeon(levels[0].levelData.numberOfRooms.Count);
            }
            else
            {
                dungeonGenerator.CreateDungeon(levels[indexCurrentLevel].levelData.numberOfRooms.Count);

            }
        }
        if (typeOfCreation == TypeOfCreation.Automata)
        {
            if (cellularAutomata == null)
            {
                cellularAutomata = new GameObject("CellularAutomataCreator").AddComponent<CellularAutomata>();
                cellularAutomataData = Resources.Load<CellularAutomataData>("New Cellular Automata Data");
                cellularAutomata.floor = cellularAutomataData.floorAutoomata;
                cellularAutomata.portal = cellularAutomataData.Portal;
                cellularAutomata.wall = cellularAutomataData.wall;
                cellularAutomata.Width = automataWidth;
                cellularAutomata.Height = automataHeight;
                cellularAutomata.nSteps = automatanSteps;
                cellularAutomata.FillRate = fillRate;

                cellularAutomata.InitAutomata();
            }
            else
            {

                cellularAutomata.InitAutomata();
            }
        }

    }
    public void NextScene()
    {
        string level = "proceduralLevel" + indexOfScene;
        if(ScenesToGo.ContainsKey(level))
        {
            SceneManager.LoadSceneAsync(level);

        }
        else
        {
            indexOfScene++;
            DeactivateAll();
            Scene newScene = SceneManager.CreateScene(level);
            ScenesToGo.Add(level, newScene);
            SceneManager.SetActiveScene(newScene);
            GenerateNextLevel();
            GenerateEvents();
            if(typeOfCreation==TypeOfCreation.Automata)
            {
                cellularAutomata = null;
            }
        }
    }
    void DeactivateAll()
    {
        Transform[] objects = GameObject.FindObjectsOfType<Transform>();
        
        foreach (Transform obj in objects)
        {
            if(obj == gameObject.transform) { continue; }

            if (ObjectToMaintainActive.Contains(obj.gameObject)) { continue; }

            if (obj.parent == null) 
            {
                obj.gameObject.SetActive(false);
            }else
            if( obj.transform.parent.name != "DontDestroyOnLoad")
                {
                    obj.gameObject.SetActive(false);
                }
        }
    }

    public void GenerateEvents()
    {
        int roomIndex = 0;
        if (indexCurrentLevel < MaxNumberOfLevels)
        {
            foreach (var room in levels[indexCurrentLevel].levelData.numberOfRooms)
            {
                //spawn enemies
                if (room.NumberOfEnemies > 0)
                {
                    for (int i = 0; i < room.NumberOfEnemies; i++)
                    {
                        switch (typeOfCreation)
                        {
                            case TypeOfCreation.RoomsConnetedWithAStar:
                                if (enemiesData == null)
                                {
                                    enemiesData = Resources.Load<EnemiesData>("EnemiesData");
                                }
                                if (room.EventsInTheRoom[0].roomEventController!=null){

                                    GameObject enemy;
                                    enemy = Instantiate(enemiesData.enemy3D,room.EventsInTheRoom[0].roomEventController.independentRoom.GetRandomFloor().asociateGameObjectWithNode.transform);
                                    enemy.transform.localPosition = Vector3.zero;
                                 }
                                break;
                            case TypeOfCreation.RogueLite:
                                GameObject enemy2 = Instantiate(enemiesData.enemy2D, dungeonGenerator.roomEvents[roomIndex].gameObject.transform);
                                enemy2.transform.localPosition = new Vector3(
                                        Random.Range(-radiusOFSpawn2d, radiusOFSpawn2d)
                                        , 0,
                                        Random.Range(-radiusOFSpawn2d, radiusOFSpawn2d)
                                        );
                                break;
                            case TypeOfCreation.Automata:
                                if (cellularAutomata.posibleEvent != null && cellularAutomata.posibleEvent.Count>0)
                                {

                                    GameObject enemy3 = Instantiate(enemiesData.enemy2D, cellularAutomata.posibleEvent[Random.Range(0, cellularAutomata.posibleEvent.Count - 1)].transform);
                                    enemy3.transform.localPosition = new Vector3(
                                       Random.Range(-radiusOFSpawn2d, radiusOFSpawn2d),0,Random.Range(-radiusOFSpawn2d, radiusOFSpawn2d));
                                }
                                break;
                            default:
                                break;  
                        }
                    
                    
                    }
                    for (int i = 0; i < room.NumberOfSpwner; i++)
                    {
                        switch (typeOfCreation)
                        {
                            case TypeOfCreation.RoomsConnetedWithAStar:
                                GameObject spawner = Instantiate(enemiesData.spwner3d, room.EventsInTheRoom[0].roomEventController.independentRoom.GetRandomFloor().asociateGameObjectWithNode.transform);
                                break;
                            case TypeOfCreation.RogueLite:
                                GameObject spwner = Instantiate(enemiesData.spwner3d, dungeonGenerator.roomEvents[roomIndex].gameObject.transform);
                                spwner.transform.localPosition = new Vector3(
                                        UnityEngine.Random.Range(-radiusOFSpawn2d, radiusOFSpawn2d)
                                        , 0,
                                        UnityEngine.Random.Range(-radiusOFSpawn2d, radiusOFSpawn2d)
                                        );
                                break;
                            case TypeOfCreation.Automata:
                                break;
                            default:
                                break;
                        }
                    }
                }
                foreach (var gameEvent in room.EventsInTheRoom)
                {
                    if (UnityEngine.Random.Range(0f,1f)<= gameEvent.ProbabilityOfConversionFloorInEvent)
                    {
                        if (gameEvent.FloorEvent != null)
                        {
                            switch (typeOfCreation)
                            {
                                case TypeOfCreation.RoomsConnetedWithAStar:
                                    if (gameEvent.roomEventController == null) { continue; }
                                    GameObject gb = Instantiate(gameEvent.FloorEvent, gameEvent.roomEventController.independentRoom.GetRandomFloor().asociateGameObjectWithNode.transform);
                                    gb.transform.localPosition = Vector3.zero;
                                    break;
                                case TypeOfCreation.RogueLite:
                                    GameObject gb2 = Instantiate(gameEvent.FloorEvent, dungeonGenerator.roomEvents[roomIndex].gameObject.transform);
                                    gb2.transform.localPosition = new Vector3(
                                        UnityEngine.Random.Range(-radiusOFSpawn2d, radiusOFSpawn2d)
                                        , 0,
                                        UnityEngine.Random.Range(-radiusOFSpawn2d, radiusOFSpawn2d)
                                        );

                                    break;
                                case TypeOfCreation.Automata:
                                    GameObject gb3 = Instantiate(gameEvent.FloorEvent, cellularAutomata.posibleEvent[Random.Range(0, cellularAutomata.posibleEvent.Count-1)].transform);
                                    break;
                                default:
                                    break;
                            }
                        }
                    
                    }
                }
            roomIndex++;
        }

        }
        else
        {
            ApplyDifficult();
            foreach (var room in levels[0].levelData.numberOfRooms)
            {
                room.NumberOfEnemies +=(int)(room.NumberOfEnemies * dificultMultiplayer);
                room.NumberOfSpwner += (int)(room.NumberOfSpwner * dificultMultiplayer);
                //spawn enemies
                if (room.NumberOfEnemies > 0)
                {
                    for (int i = 0; i < room.NumberOfEnemies; i++)
                    {
                        switch (typeOfCreation)
                        {
                            case TypeOfCreation.RoomsConnetedWithAStar:
                                GameObject enemy = Instantiate(enemiesData.enemy3D, room.EventsInTheRoom[0].roomEventController.independentRoom.GetRandomFloor().asociateGameObjectWithNode.transform);
                                enemy.transform.localPosition = Vector3.zero;
                                break;
                            case TypeOfCreation.RogueLite:
                                GameObject enemy2 = Instantiate(enemiesData.enemy3D, dungeonGenerator.roomEvents[roomIndex].gameObject.transform);
                                enemy2.transform.localPosition = new Vector3(
                                        UnityEngine.Random.Range(-radiusOFSpawn2d, radiusOFSpawn2d)
                                        , 0,
                                        UnityEngine.Random.Range(-radiusOFSpawn2d, radiusOFSpawn2d)
                                        );
                                break;
                            case TypeOfCreation.Automata:
                                break;
                            default:
                                break;
                        }


                    }
                    for (int i = 0; i < room.NumberOfSpwner; i++)
                    {
                        switch (typeOfCreation)
                        {
                            case TypeOfCreation.RoomsConnetedWithAStar:
                                GameObject spawner = Instantiate(enemiesData.spwner3d, room.EventsInTheRoom[0].roomEventController.independentRoom.GetRandomFloor().asociateGameObjectWithNode.transform);
                                break;
                            case TypeOfCreation.RogueLite:
                                GameObject spwner = Instantiate(enemiesData.spwner3d, dungeonGenerator.roomEvents[roomIndex].gameObject.transform);
                                spwner.transform.localPosition = new Vector3(
                                        UnityEngine.Random.Range(-radiusOFSpawn2d, radiusOFSpawn2d)
                                        , 0,
                                        UnityEngine.Random.Range(-radiusOFSpawn2d, radiusOFSpawn2d)
                                        );
                                break;
                            case TypeOfCreation.Automata:
                                break;
                            default:
                                break;
                        }
                    }
                }
                foreach (var gameEvent in room.EventsInTheRoom)
                {
                    if (UnityEngine.Random.Range(0f, 1f) <= gameEvent.ProbabilityOfConversionFloorInEvent)
                    {
                        if (gameEvent.FloorEvent != null)
                        {
                            switch (typeOfCreation)
                            {
                                case TypeOfCreation.RoomsConnetedWithAStar:
                                    GameObject gb = Instantiate(gameEvent.FloorEvent, gameEvent.roomEventController.independentRoom.GetRandomFloor().asociateGameObjectWithNode.transform);
                                    gb.transform.localPosition = Vector3.zero;
                                    break;
                                case TypeOfCreation.RogueLite:
                                    GameObject gb2 = Instantiate(gameEvent.FloorEvent, dungeonGenerator.roomEvents[roomIndex].gameObject.transform);
                                    gb2.transform.localPosition = new Vector3(
                                        UnityEngine.Random.Range(-radiusOFSpawn2d, radiusOFSpawn2d)
                                        , 0,
                                        UnityEngine.Random.Range(-radiusOFSpawn2d, radiusOFSpawn2d)
                                        );

                                    break;
                                case TypeOfCreation.Automata:
                                    break;
                                default:
                                    break;
                            }
                        }

                    }
                }
                roomIndex++;
            }
        }
        indexCurrentLevel++;
    }

}
