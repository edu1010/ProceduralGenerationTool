using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
 
public class DungeonGenerator :ScriptableObject
{
    private Room[,] Grid;
    [SerializeField]
    private int _numberOfRooms;
    GameObject _parent;
    AvalibleRooms avalibleRooms;
    bool portalCreated = false;
    public List<RoomEventController> roomEvents=new();
    void InitGrid()
    {
        avalibleRooms = Resources.Load<AvalibleRooms>("Rooms/New Avalible Rooms");
        int gridSize = 3 * _numberOfRooms;
        Grid = new Room[gridSize, gridSize];
    }
    void ShowDungeon()
    {
        for (int i = 0; i < Grid.GetLength(0); i++)
        {
            for (int j = 0; j < Grid.GetLength(1); j++)
            {
                if (Grid[i, j] != null)
                {
                    //Debug.Log("deberia de hacerse un objeto");
                    //Instantiate(RoomPrefab, new Vector2(i, j),Quaternion.identity);
                    //Debug.Log("Instanciar " + fileName);
                    if(( Random.Range(1,10) < 5) && portalCreated == false  || ( j < Grid.GetLength(1) -2 && portalCreated == false))
                    {
                        CreateRoom(Grid[i, j], true);
                    }
                    else
                    {
                        CreateRoom(Grid[i, j], false);
                    }


                }
            }
        }
    }

    public void CreateRoom(Room actualRoom,bool teleport)
    {
        bool n = false, s = false, e = false, w = false;
        foreach (directions dir in actualRoom.Neighbours.Keys)
        {
            switch (dir)
            {
                case directions.N:
                    n = true;
                    break;
                case directions.S:
                    s = true;
                    break;
                case directions.E:
                    e = true;
                    break;
                case directions.W:
                    w = true;
                    break;
                default:
                    break;
            }
        }
        avalibleRooms.GetRoom(n, s, e, w, teleport);
        GameObject room = Instantiate(avalibleRooms.GetRoom(n, s, e, w, false), new Vector2(actualRoom.x * 10, actualRoom.y * 10), Quaternion.identity);
        room.transform.parent = _parent.transform;
        RoomEventController roomEvent = room.gameObject.AddComponent<RoomEventController>();
        roomEvent.room = actualRoom;
        roomEvents.Add(roomEvent);

    }

    public Vector2 ObtenerCuevaAbajo()
    {
        for (int i = Grid.GetLength(0); i < 0; i++)
        {
            for (int j = Grid.GetLength(1); j < 0 ; j++)
            {
                if (Grid[i, j] != null)
                {
                   return new Vector2(i, j);
                }
            }
        }
        return new Vector2(0, 0);

    }

    void GenerateDungeon()
    {
        InitGrid();
        Vector2Int coord = new Vector2Int(Grid.GetLength(0) / 2 - 1, Grid.GetLength(1) / 2 - 1);// Da el  centro del mapa, grid length 0 devuelve el lenghh del primer conjunto de variables de y el 1 del segundo de la matriz
        Queue<Room> roomsToCreate = new Queue<Room>();

        Room firstRoom = new Room(coord);
        roomsToCreate.Enqueue(firstRoom);

        List<Room> createdRooms = new List<Room>();//Lista para saber si esta creado o no
        //createdRooms.Add(firstRoom);

        AddRooms(roomsToCreate, createdRooms);
        ConnectNeighbours(createdRooms);
    }

    private void ConnectNeighbours(List<Room> createdRooms)
    {
        foreach(var room in createdRooms)
        {
            List<Vector2Int> neighborCoordinates = room.getNeighbourCoordinates();
            foreach(Vector2Int coordinate in neighborCoordinates)
            {
                Room neighbor = this.Grid[coordinate.x, coordinate.y];
                if(neighbor != null)
                {
                    room.Connect(neighbor);//Puede entrar entre 1 y 4 veces, depende de los vecinos que tenga
                }
            }
        }
    }

    private void AddRooms(Queue<Room> roomsToCreate, List<Room> createdRooms)
    {
        //List<Room> 
        while(roomsToCreate.Count>0 && createdRooms.Count < _numberOfRooms)
        {
            Room currentRoom = roomsToCreate.Dequeue();//Cogemos el primero de la lista y esa es la habitación con la que trabajamos, dequeque tambien lo saca de esta lista
            Grid[currentRoom.x, currentRoom.y] = currentRoom;//Asi podemos saber en el grid si hay una habitacion o no, podriamos guardar 0 o 1  pero guardando la habitación podemos mirar si tiene vecinos luego etc
            createdRooms.Add(currentRoom);
            AddNeighbours(currentRoom, roomsToCreate);
        }
    }

    private void AddNeighbours(Room currentRoom, Queue<Room> roomsToCreate)
    {
        List<Vector2Int> availableNeighbors = GetAvalibleNeighbors(currentRoom);

        int numberOfNeighbors = (int)UnityEngine.Random.Range(1, availableNeighbors.Count);

        CreateNeighbors(availableNeighbors, numberOfNeighbors, roomsToCreate);
    }


    private List<Vector2Int> GetAvalibleNeighbors(Room currentRoom)//Miraremos los alrededores y si no esta creado añadimos esa coord para luego decidir si creearla
    {
        List<Vector2Int> possibleNeighboursCoords = currentRoom.getNeighbourCoordinates();//Miramos cuales son las cordenadas de los vecinos de arriba abajo etc
        List<Vector2Int> availableNeighboursCoords = new List<Vector2Int>();// lista con de la lista anterior cueles no existen aun
        foreach(Vector2Int coordinate in possibleNeighboursCoords)
        {
            //Bordes
            if(this.Grid[coordinate.x,coordinate.y] == null)
            {
                availableNeighboursCoords.Add(coordinate);
            }
        }
        return availableNeighboursCoords;
    }

    private void CreateNeighbors(List<Vector2Int> availableNeighbors, int numberOfNeighbors, Queue<Room> roomsToCreate)
    {//en un bucle
        //Cogemos uno de la lista,
        //Add to queque
        //remove from possible list
        for(int i = 0; i < numberOfNeighbors; i++)
        {
            int chosen = Random.Range(0, availableNeighbors.Count);
            Vector2Int choosenNeighbour = availableNeighbors[chosen];
            roomsToCreate.Enqueue( new Room(choosenNeighbour));
            availableNeighbors.Remove(choosenNeighbour);

        }
    }


    /*private void Start()
    {
        GenerateDungeon();
        ShowDungeon();
        //PlayerManager.GeneratePlayer();
    }*/
    public void CreateDungeon(int numberOfRooms)
    {
        _parent = new GameObject("levelDungeon");
        _numberOfRooms = numberOfRooms;
        GenerateDungeon();
        ShowDungeon();
    }

    
}