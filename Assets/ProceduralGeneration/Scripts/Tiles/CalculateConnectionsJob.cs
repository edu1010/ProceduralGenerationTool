using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using UnityEngine;

[BurstCompile] //no admite listas
public struct  CalculateConnectionsJob : IJob
{
         NativeArray<bool> _connectionUp;
         NativeArray<bool> _connectionDown;
         NativeArray<bool> _connectionLeft;
         NativeArray<bool> _connectionRight;
         private NativeArray<Vector2> _neighbourUp;
         private NativeArray<Vector2> _neighbourDown;
         private NativeArray<Vector2> _neighbourLeft;
         private NativeArray<Vector2> _neighbourRight;
         
         private Vector3 _position;
         private bool _is3d;
         private int _verticalSizeGrid;
         private int _horizontalSizeGrid;
         public CalculateConnectionsJob(
             Vector3 position, 
             bool is3d, 
             NativeArray<bool> connectionUp, 
             NativeArray<bool> connectionDown, 
             NativeArray<bool> connectionLeft,
             NativeArray<bool> connectionRight, 
             NativeArray<Vector2> neighbourUp, 
             NativeArray<Vector2> neighbourDown,
             NativeArray<Vector2> neighbourLeft, 
             NativeArray<Vector2> neighbourRight,
             int verticalSizeGrid, 
             int horizontalSizeGrid
             )
         {
             _position = position;
             _is3d = is3d;
             _connectionUp = connectionUp;
             _connectionDown = connectionDown;
             _connectionLeft = connectionLeft;
             _connectionRight = connectionRight;
             _neighbourUp = neighbourUp;
             _neighbourDown = neighbourDown;
             _neighbourLeft = neighbourLeft;
             _neighbourRight = neighbourRight;
             _verticalSizeGrid = verticalSizeGrid;
             _horizontalSizeGrid = horizontalSizeGrid;

         }
    public void Execute()
    {
            if (!(_position.x + 1 >= _verticalSizeGrid))
            {
                _connectionRight[0] = true;
                _neighbourRight[0]=new Vector2(_position.x+1,_position.z);
            }
            if (!(_position.x -1 < 1))
            {
                _connectionLeft[0] = true;
                _neighbourLeft[0] = new Vector2(_position.x - 1, _position.z);
            }
            if (!(_position.z+1 >= _horizontalSizeGrid))
            {
                _connectionUp[0] = true;
                _neighbourUp[0] = new Vector2(_position.x, _position.z + 1);
            }
            if (!(_position.z-1 < 1))
            {
                _connectionDown[0] = true;
                _neighbourDown[0] = new Vector2(_position.x, _position.z - 1);
            }
    }
}