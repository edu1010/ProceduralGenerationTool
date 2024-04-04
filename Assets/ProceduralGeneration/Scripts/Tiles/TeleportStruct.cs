using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TeleportStruct 
{
    public IndependentRoom startRoom;
    public IndependentRoom roomToConect;

    public TeleportStruct(IndependentRoom startRoom, IndependentRoom roomToConect) : this()
    {
        this.startRoom = startRoom;
        this.roomToConect = roomToConect;
    }
}
