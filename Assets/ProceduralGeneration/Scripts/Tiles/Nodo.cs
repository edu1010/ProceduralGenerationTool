using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using UnityEngine;
using Unity.Jobs;
using System.Threading.Tasks;

//[ExecuteAlways]
public class Nodo 
{
    public int x, y, z;
    public Vector3 position;
    public List<Nodo> conections;
    public bool is3d;
    public bool _conectionUP, _conectionDown, _conectionLeft, _conectionRight= false;
    public Nodo nodoUP, nodoDown, nodoLeft, nodoRight;
    public bool isEmpty = true;
    public Guid Id { get; set; }
    public Nodo[,] _grid;
    public int _verticalSizeGrid;//lo hago publico para el job
    public int _horizontalSizeGrid;
    public TIleToCollapse TiIleToCollapse;
    public float cost = 1f;
    public GameObject asociateGameObjectWithNode;
    public NodeInfoToCollapse nodeData;

    public Nodo(int x, int y, int z,bool is3d,ref Nodo[,]grid,int verticalSizeGrid,int horizontalSizeGrid  )
    {
        _grid = grid;
        _verticalSizeGrid = verticalSizeGrid;
        _horizontalSizeGrid = horizontalSizeGrid;
        this.x = x;
        this.y = y;
        this.z = z;
        position = new Vector3(x, y, z);
        this.is3d = is3d;
        conections = new List<Nodo>();
        Id = Guid.NewGuid();
    }

    NativeArray<bool>    connectionUpResult     = new NativeArray<bool>(1, Allocator.Persistent);
    NativeArray<bool>    connectionDownResult   = new NativeArray<bool>(1, Allocator.Persistent);
    NativeArray<bool>    connectionLeftResult  = new NativeArray<bool>(1, Allocator.Persistent);
    NativeArray<bool>    connectionRightResult = new NativeArray<bool>(1, Allocator.Persistent);
    NativeArray<Vector2> neighbourUpResult     = new NativeArray<Vector2>(1, Allocator.Persistent);
    NativeArray<Vector2> neighbourDownResult   = new NativeArray<Vector2>(1, Allocator.Persistent);
    NativeArray<Vector2> neighbourLeftResult   = new NativeArray<Vector2>(1, Allocator.Persistent);
    NativeArray<Vector2> neighbourRightResult   = new NativeArray<Vector2>(1, Allocator.Persistent);


    public void CalculateConections()
    {
        
    
        CalculateConnectionsJob job = new CalculateConnectionsJob(position, is3d, connectionUpResult, connectionDownResult, connectionLeftResult, connectionRightResult,
            neighbourUpResult, neighbourDownResult, neighbourLeftResult, neighbourRightResult,
            _verticalSizeGrid, _horizontalSizeGrid
        );
        JobHandle handle = job.Schedule();
        //MonoBehaviour.StartCoroutine(OnCompleteJob(handle));
        OnCompleteJob(handle);
    }

    public void OnCompleteJob(JobHandle handle)
    {        
        handle.Complete();

        var tcs = new TaskCompletionSource<object>();

        // Ejecuta la rutina en un hilo de trabajo separado
        Task.Run(() =>
        {
            // Espera hasta que se complete el trabajo
            while (!handle.IsCompleted)
            {
                // Espera 1 milisegundo para evitar un bloqueo innecesario del subproceso
                Thread.Sleep(1);
                Debug.Log("espero");
            }

            // Realiza las operaciones necesarias con los resultados del trabajo
            _conectionUP = connectionUpResult[0];
            if (_conectionUP)
            {
                conections.Add(_grid[(int)neighbourUpResult[0].x,(int)neighbourUpResult[0].y]);
                nodoUP = _grid[(int)neighbourUpResult[0].x, (int)neighbourUpResult[0].y];
            }

            _conectionDown = connectionDownResult[0];
            if (_conectionDown)
            {
                conections.Add(_grid[(int)neighbourDownResult[0].x, (int)neighbourDownResult[0].y]);
                nodoDown = _grid[(int)neighbourDownResult[0].x, (int)neighbourDownResult[0].y];
            }
        
            _conectionLeft = connectionLeftResult[0];
            if (_conectionLeft)
            {
                conections.Add(_grid[(int)neighbourLeftResult[0].x, (int)neighbourLeftResult[0].y]);
                nodoLeft = _grid[(int)neighbourLeftResult[0].x, (int)neighbourLeftResult[0].y];
            }

            _conectionRight = connectionRightResult[0];
            if (_conectionRight)
            {
                conections.Add(_grid[(int)neighbourRightResult[0].x, (int)neighbourRightResult[0].y]);
                nodoRight = _grid[(int)neighbourRightResult[0].x, (int)neighbourRightResult[0].y];
            }

            // Limpia los recursos no administrados
            connectionUpResult.Dispose();
            connectionDownResult.Dispose();
            connectionLeftResult.Dispose();
            connectionRightResult.Dispose();
            neighbourUpResult.Dispose();
            neighbourDownResult.Dispose();
            neighbourLeftResult.Dispose();
            neighbourRightResult.Dispose();

            // Indica que la tarea se completó con éxito
            tcs.SetResult(null);
        });

        // Devuelve la tarea personalizada
        //return tcs.Task;
    }
    IEnumerator  OnCompleteJobIenumarator( JobHandle handle)
    {
        while (!handle.IsCompleted)
        {
            yield return null;
        }

        _conectionUP = connectionUpResult[0];
        if (_conectionUP)
            conections.Add(_grid[(int)neighbourUpResult[0].x,(int)neighbourUpResult[0].y]);

        _conectionDown = connectionDownResult[0];
        if (_conectionDown)
            conections.Add(_grid[(int)neighbourDownResult[0].x, (int)neighbourDownResult[0].y]); 
        
        _conectionLeft = connectionLeftResult[0];
        if (_conectionLeft)
            conections.Add(_grid[(int)neighbourLeftResult[0].x, (int)neighbourLeftResult[0].y]);

        _conectionRight = connectionRightResult[0];
        if (_conectionRight)
            conections.Add(_grid[(int)neighbourRightResult[0].x, (int)neighbourRightResult[0].y]);
        
        connectionUpResult.Dispose();
        connectionDownResult.Dispose();
        connectionLeftResult.Dispose();
        connectionRightResult.Dispose();
        neighbourUpResult.Dispose();
        neighbourDownResult.Dispose();
        neighbourLeftResult.Dispose();
        neighbourRightResult.Dispose();
    }
    public void CalculateConectionsWithoutMultiThreding()
    {
        NativeArray<bool> l_conectionRight;
        if (is3d)//xz y=0
        {
            //x vertical
            if (!(x + 1 >= _verticalSizeGrid))
            {
                _conectionRight = true;
                conections.Add(_grid[x+1,z]);
            }
            if (!(x -1 < 1))
            {
                _conectionLeft = true;
                conections.Add(_grid[x-1,z]);
            }
            if (!(z+1 >= _horizontalSizeGrid))
            {
                _conectionUP = true;
                conections.Add(_grid[x,z+1]);
            }
            if (!(z-1 < 1))
            {
                _conectionDown = true;
                conections.Add(_grid[x,z-1]);
            }
        }
        else
        {
            
        }

    }
    public void ShowConections()
    {
        string str="";
        foreach (var var in conections)
        {
            str = str + var.position + " ";
        }
        Debug.Log(str);
    }
    
    
    public override bool Equals(object obj) 
    {
        if (obj == null || !(obj is Nodo)) {
            return false;
        }

        Nodo other = (Nodo)obj;
        return this.Id == other.Id;
    }

    public override int GetHashCode() 
    {
        return this.Id.GetHashCode();
    }

    public void Reset()
    {
        isEmpty = true;
        cost= 1;
        if(nodeData!= null)
        {
            nodeData.Reset();
        }
    }
    
}

