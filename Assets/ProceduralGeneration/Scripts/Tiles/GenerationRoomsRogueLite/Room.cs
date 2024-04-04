using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room 
{
    private Vector2Int coord;
    public int x => coord.x;
    public int y => coord.y;

    public Dictionary<directions, Room> Neighbours;
    

    public Room(Vector2Int coord)
    {
        this.coord = coord;
    }

    public List<Vector2Int> getNeighbourCoordinates()
    {

        List<Vector2Int> coords = new List<Vector2Int>();
        coords.Add(new Vector2Int(this.x, this.y - 1));//abajo
        coords.Add(new Vector2Int(this.x, this.y + 1));//arriba
        coords.Add(new Vector2Int(this.x-1, this.y));//Izquierda
        coords.Add(new Vector2Int(this.x+1, this.y ));//Derecha
        return coords;
    }

    internal  void Connect(Room neighbor)
    {
        directions direction = directions.N;
        if(neighbor.y < this.y)
        {
            direction = directions.S;
        }
        if(neighbor.y > this.y)
        {
            direction = directions.N;
        } 
        if(neighbor.x > this.x)
        {
            direction = directions.W;
        }
        if(neighbor.x < this.x)
        {
            direction = directions.E;
        }

        if(Neighbours == null)
        {
            Neighbours = new Dictionary<directions, Room>();
        }
        Neighbours.Add(direction, neighbor);
        
    }
    
}
public enum directions
{
    N,
    S,
    E,
    W
}
