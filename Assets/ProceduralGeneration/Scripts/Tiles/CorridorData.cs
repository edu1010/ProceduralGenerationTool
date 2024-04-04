using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorData : MonoBehaviour
{
   [SerializeField] private IndependentRoom _startRoom;
   [SerializeField] private IndependentRoom _roomToConect;
    public IndependentRoom GetStartRoom()
    {
        return _startRoom;
    }
    public void SetStarRoom(IndependentRoom value)
    {
        if (_startRoom!=null &&_startRoom.corridorConectedToThisRoom.Contains(this))
        {
            _startRoom.corridorConectedToThisRoom.Remove(this);
        }
        _startRoom = value;
        _startRoom.corridorConectedToThisRoom.Add(this);
    }
    public IndependentRoom GetRoomToConect() 
    {
        return _roomToConect;
    }
    public void SetRoomToConect(IndependentRoom value) 
    {
        if (_roomToConect != null &&_roomToConect.corridorConectedToThisRoom.Contains(this))
        {
            _roomToConect.corridorConectedToThisRoom.Remove(this);
        }
        _roomToConect = value;
        _roomToConect.corridorConectedToThisRoom.Add(this);
    }
}
