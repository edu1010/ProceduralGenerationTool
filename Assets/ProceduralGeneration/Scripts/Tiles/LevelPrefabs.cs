using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Level Prefabs", menuName = "Level Prefabs")]
public class LevelPrefabs : ScriptableObject
{
    public GameObject[] Floor;
    public GameObject[] Wall;
    public GameObject[] Corner;
    public GameObject[] Door;
    public GameObject[] Corridor;
    public GameObject[] CorridorCorner;
    public GameObject GetRandomFloor()
    {
        return Floor[Random.Range(0, Floor.Length)];
    }

    public GameObject GetRandomWall()
    {
        return Wall[Random.Range(0, Wall.Length)];
    }

    public GameObject GetRandomCorner()
    {
        return Corner[Random.Range(0, Corner.Length)];
    }

    public GameObject GetRandomDoor()
    {
        return Door[Random.Range(0, Door.Length)];
    }

    public GameObject GetRandomCorridor()
    {
        return Corridor[Random.Range(0, Corridor.Length)];
    }
    public GameObject GetRandomCorridorCorner()
    {
        return CorridorCorner[Random.Range(0, CorridorCorner.Length)];
    }

}
