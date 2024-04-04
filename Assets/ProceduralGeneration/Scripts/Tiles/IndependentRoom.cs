using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
[ExecuteAlways]
public class IndependentRoom : MonoBehaviour
{

    public Guid Id= Guid.NewGuid();
    private List<Nodo> _walls =new List<Nodo>();
    private List<Nodo> _floor =new List<Nodo>();
    private List<Nodo> _corner =new List<Nodo>();

    public List<CorridorData> corridorConectedToThisRoom= new List<CorridorData>();
    public List<TeleportStruct> portalsData = new List<TeleportStruct>();
    public RoomEventController roomEventController;
    private void OnEnable()
    {
        RandomRoomPlacementManager.OnAddRoom?.Invoke(this);
        if(roomEventController == null)
        {
            roomEventController = gameObject.AddComponent<RoomEventController>();
            roomEventController.independentRoom = this;
        }
    }
    private void Start()
    {
        
    }

    bool _destroidByEditor = true;
    bool _Died=false;
    bool _OnDestroyCall=false;
    public void DestroyRoom()
    {
        if (_Died) { return; }
        _Died=true;
        _destroidByEditor = false;
        foreach (var nodo in _walls)
        {
            nodo.Reset();
        }
        foreach (var nodo in _floor)
        {
            nodo.Reset();
        }
        foreach (var nodo in _corner)
        {
            nodo.Reset();
        }
        if(!_OnDestroyCall)
            DestroyImmediate(gameObject);
    }
    private void OnDestroy()
    {
        if (_destroidByEditor)
        {
            _OnDestroyCall=true;
            DestroyRoom();
        }
        _Died = true;
        RandomRoomPlacementManager.OnDeleteRoom?.Invoke(this);
    }
    
    public void AddWall(Nodo wall)
    {
        _walls.Add(wall);
    }

    public void AddFloor(Nodo floor)
    {
        _floor.Add(floor);
    }

    public void AddCorner(Nodo corner)
    {
        _corner.Add(corner);
    }
    
    
    public List<Nodo> GetWalls()
    {
        return _walls;
    }

    public List<Nodo> GetFloor()
    {
        return _floor;
    }

    public List<Nodo> GetCorner()
    {
        return _corner;
    }

    public Nodo GetRandomWall()
    {
        if (_walls.Count == 0) return null;

        int randomIndex = Random.Range(0, _walls.Count-1);
        
        return _walls[randomIndex];
    }

    public Nodo GetRandomFloor()
    {
        if (_floor.Count == 0) return null;

        int randomIndex = Random.Range(0, _floor.Count-1);
        return _floor[randomIndex];
    }
    public void AddEventToRoom(GameObject eventObject)
    {
        GameObject e = Instantiate(eventObject, GetRandomFloor().asociateGameObjectWithNode.transform);
        e.transform.localPosition = Vector3.zero;
    }
    
}
