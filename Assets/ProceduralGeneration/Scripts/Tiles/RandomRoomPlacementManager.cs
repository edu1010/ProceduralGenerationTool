using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TilesConPathFinding.Astar;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
[RequireComponent(typeof(AStarCreatePath))]
[ExecuteAlways]
public class RandomRoomPlacementManager : MonoBehaviour
{
    public int maxHeight = 10;
    public int maxWidth = 10;
    public int minHeight = 3;
    public int minWidth = 3;
    
    public int numberOfRooms = 5;
    private int _maxNumberOfIntents=4;

    private AStarCreatePath createPath;
    public GameObject corridorGameobject;
    GridGenerator gridGenerator;
    private List<IndependentRoom> _independentRooms = new List<IndependentRoom>();
    public List<IndependentRoom> IndependentRooms => _independentRooms.Where(x => x.GetWalls().Count > 1).ToList();
    public static Action<IndependentRoom> OnAddRoom;
    public static Action<IndependentRoom> OnDeleteRoom;
    public GameObject portal;
    [SerializeField] private TeleportPlayer _portal;

    List<IndependentRoom> roomsToConnect;
    GameObject LevelPrefab;
    public void Init(int MaxHeight,
                     int maxWidth ,
                     GridGenerator gridGenerator,
                     int minHeight,
                     int minWidth, 
                     int numberOfRooms
                     ,GameObject corridorGameobject
                     , GameObject portal
        )
    {
        this.maxHeight     = MaxHeight    ;
        this.maxWidth      = maxWidth     ;
        this.minHeight     = minHeight    ;
        this.minWidth      = minWidth     ;
        this.numberOfRooms = numberOfRooms;
        this.corridorGameobject = corridorGameobject;
        this.portal = portal;
        _portal= portal.GetComponent<TeleportPlayer>();
        this.gridGenerator = gridGenerator;


    }
    void OnEnable()
    {
        OnAddRoom += AddRoom;
        OnDeleteRoom += DeleteRoom;
        //GridGenerator.OnComplete += CreateNecesaryRooms;
        //GridGenerator.Instance.GenerateGrid3D();
        createPath= GetComponent<AStarCreatePath>();

    }

    private void OnDisable()
    {
        OnAddRoom -= AddRoom;
        OnDeleteRoom -= DeleteRoom;
        //GridGenerator.OnComplete -= CreateNecesaryRooms;


    }
    private void Start(){}
    private void DeleteRoom(IndependentRoom room)
    {
        _independentRooms.Remove(room);

    }

    private void AddRoom(IndependentRoom room)
    {
        _independentRooms.Add(room);

        if (roomsToConnect != null)
        {
            if(room.GetWalls().Count > 1)
                roomsToConnect.Add(room);

        }
    }

    public void CreateNecesaryRooms()
    {
        for (int i = 0; i < numberOfRooms; i++)
        {
            int width=Random.Range(minWidth,maxWidth); 
            int height=Random.Range(minHeight,maxHeight);
            int currentNomberOfTrys = 0;
            while (!RoomGenerator.Instance.GenerateRandomRoom(width,height, gridGenerator))
            {
                currentNomberOfTrys++;
                if(currentNomberOfTrys>_maxNumberOfIntents){return;}
            }
        }

       ConnectRoom(true);
       GenerateNavmeshForRooms();
    }


    private void CreateNecesaryRooms(int numberofRooms)
    {
        for (int i = 0; i < numberofRooms; i++)
        {
            int width=Random.Range(minWidth,maxWidth); 
            int height=Random.Range(minHeight,maxHeight);
            int currentNomberOfTrys = 0;
            while (!RoomGenerator.Instance.GenerateRandomRoom(width,height,gridGenerator))
            {
                currentNomberOfTrys++;
                if(currentNomberOfTrys>_maxNumberOfIntents){return;}
            }
        }
        GenerateNavmeshForRooms();
       //ConnectRooms();
    }
    public void GenerateNavmeshForRooms()
    {
        var gb = new GameObject();
        var nav = gb.AddComponent<NavMeshSurface>();
        nav.BuildNavMesh();
    }
    
    public void ConnectRoom(bool internalCall=false)
    {
        if (internalCall || LevelPrefab == null) 
            LevelPrefab = new GameObject("Level");
        
        roomsToConnect = _independentRooms;
        roomsToConnect = roomsToConnect.Where(x => x.GetWalls().Count > 1).ToList();

        int maxTryOfConnectRooms = 10;
        int currentTryOfConnectRooms = 0;
        while (roomsToConnect.Count > 1 )
        {

            IndependentRoom startRoom = roomsToConnect[Random.Range(0, roomsToConnect.Count)];
            IndependentRoom roomToConect = roomsToConnect[Random.Range(0, roomsToConnect.Count)];
            while(startRoom == roomToConect)
            {
                startRoom = roomsToConnect[Random.Range(0, roomsToConnect.Count)];
                roomToConect = roomsToConnect[Random.Range(0, roomsToConnect.Count)];
            }
            Nodo start = startRoom.GetRandomWall();
            Nodo finish = roomToConect.GetRandomWall();


            while(start.nodeData.GetType()==TypeOfNodes.Door || finish.nodeData.GetType()==TypeOfNodes.Door)
            {
                start = startRoom.GetRandomWall();
                finish = roomToConect.GetRandomWall();

            }
            List<Nodo> nodoPath = createPath.Inizialize(
            start,
            finish);
            
            bool shouldContinue =(!CheckPath(nodoPath, start,finish));
         
            if (shouldContinue)
            {
                currentTryOfConnectRooms++;
                if (currentTryOfConnectRooms > maxTryOfConnectRooms) 
                {

                    //chekeo si   esa habitacion no conectada puede conectar con otra ya conectada

                    for (int i = 0; i < _independentRooms.Count; i++)
                    {
                        if (_independentRooms[i] != roomToConect)
                        {
                            Nodo s = _independentRooms[i].GetRandomWall();
                            Nodo f = roomToConect.GetRandomWall();

                           nodoPath = createPath.Inizialize(s,f);
                           if(CheckPath(nodoPath, s, f))
                           {
                               start = s;
                               finish = f;
    
                                shouldContinue = false;
                                break;
                            }
                        }
                    }

                }
                if (shouldContinue)
                {
                    Debug.Log("delete room");
                    CreateTeleportInRooms(startRoom, roomToConect);
                    roomsToConnect.Remove(startRoom);

                    startRoom.transform.parent = LevelPrefab.transform;
                    roomToConect.transform.parent = LevelPrefab.transform;
                    //DeleteRoomAndRecreate(roomToConect);
                    continue;
                }
            }
          
                               
            Debug.Log(shouldContinue + " DEBE SER FALSO");
            GameObject corridorParent = new GameObject("Corridor_");
            CorridorData corridorData = corridorParent.AddComponent<CorridorData>();
            Debug.Log("Corridor created");
            corridorData.SetStarRoom(startRoom);
            corridorData.SetRoomToConect(roomToConect);


            start.nodeData.ForceCollapse(TypeOfNodes.Door);
            finish.nodeData.ForceCollapse(TypeOfNodes.Door);

            corridorParent.transform.parent = LevelPrefab.transform;
            startRoom.transform.parent = LevelPrefab.transform;
            roomToConect.transform.parent = LevelPrefab.transform;



            foreach (var n in nodoPath)
            {
                if(n.nodeData==null || n.nodeData.GetType()!=TypeOfNodes.Door)
                {
                    GameObject gb= gridGenerator.InstantiatePrefab(corridorGameobject, n);
                    gb.transform.parent = corridorParent.transform;
                    n.isEmpty= false;
                    n.cost = 200;
                }
            } 
            foreach (var n in nodoPath)
            {
                if(n.nodeData==null || n.nodeData.GetType()!=TypeOfNodes.Door)
                {
                    foreach (var nodoToModifyCost in n.conections)
                    {
                        nodoToModifyCost.cost = 200;
                    }
                    n.nodeData.ForceCollapse(TypeOfNodes.Corridor);
                    n.asociateGameObjectWithNode.name = n.x + " " + n.z;
                }
            }




            roomsToConnect.Remove(startRoom);
            _independentRooms.Remove(startRoom);
            nodoPath.Clear();

        }


    }

    private void CreateTeleportInRooms(IndependentRoom startRoom, IndependentRoom roomToConect)
    {
        TeleportPlayer p1= Instantiate(_portal, startRoom.GetRandomFloor().asociateGameObjectWithNode.transform,false);
        TeleportPlayer p2 = Instantiate(_portal, roomToConect.GetRandomFloor().asociateGameObjectWithNode.transform,false);
        p1.transform.localPosition = Vector3.zero;
        p2.transform.localPosition = Vector3.zero;
        startRoom.portalsData.Add(new TeleportStruct(startRoom, roomToConect));
        roomToConect.portalsData.Add(new TeleportStruct(roomToConect, startRoom));

        p1.nextLocation = p2.pointOfAparition;
        p2.nextLocation = p1.pointOfAparition;
    }

    public bool CheckPath(List<Nodo> nodoPath,Nodo start,Nodo finish)
    {
        bool validPath = true;
        foreach (var n in nodoPath)
        {
            if (n != start && n != finish)
            {
                if (!(n.isEmpty && n.cost == 1))
                {
                    validPath = false;
                    break;
                }
            }
        }
        return validPath;

    }

    public void DeleteRoomAndRecreate(IndependentRoom roomToDestroy)
    {
        CreateNecesaryRooms(1);
        return;

        roomsToConnect.Remove(roomToDestroy);

        int roomsDestroyed = 1;//empiezo en uno pq sumo el resto conectadas a el luego
        foreach (var corridor in roomToDestroy.corridorConectedToThisRoom)
        {
            if(roomToDestroy.Id!= corridor.GetStartRoom().Id)
            {
                roomsToConnect.Remove(corridor.GetStartRoom());
                corridor.GetStartRoom().DestroyRoom();

            }

            if(roomToDestroy.Id != corridor.GetRoomToConect().Id)
            {
                roomsToConnect.Remove(corridor.GetRoomToConect());
                corridor.GetRoomToConect().DestroyRoom();
            }
            
            roomsDestroyed++;
            //StartCoroutine(DebugDestroy(corridor));
            if(corridor!=null)
                DestroyImmediate(corridor.gameObject);
        }

        CreateNecesaryRooms(roomsDestroyed);
        roomToDestroy.DestroyRoom();
    }
    IEnumerator DebugDestroy(CorridorData corridor)
    {
        if (corridor != null)
            DestroyImmediate(corridor.gameObject);
        yield return null;
    }
}
