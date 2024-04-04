using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.Jobs;
[System.Serializable]
[ExecuteAlways]
public class GridGenerator :ScriptableObject
{
    [SerializeField]
    private bool _2d = false;
    private Nodo [,] _grid;
    public Nodo[,] Grid => _grid;
   
    public int horizontalSize = 100;
    public int verticalSize = 50;
    public GameObject floor;
    private Vector3 floorSize;
    private List<Nodo> nodeList= new List<Nodo>();
    //public static GridGenerator Instance;
    private GameObject parent;
    public static Action OnComplete;

    public bool manualEditor=false;
    private void OnEnable()
    {
        /*
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            floorSize = floor.GetComponent<Renderer>().bounds.size;
        }*/
    }
    public GridGenerator()
    {
       // floorSize = floor.GetComponent<Renderer>().bounds.size;

    }
    // Start is called before the first frame update
    void Start()
    {
        //nodeList = new List<Nodo>();
        //GenerateGrid3D();
        
    }
    public void GenerateGrid3D(GameObject floorDeku)
    {
        floor = floorDeku;
        floorSize = floor.GetComponent<Renderer>().bounds.size;
        

        parent = new GameObject("ParentGrid");
        
        NativeArray<JobHandle> jobHandleArray = new NativeArray<JobHandle>(verticalSize*horizontalSize,Allocator.Persistent);


        _grid = new Nodo[verticalSize, horizontalSize];
        int x, z; 
       // floorSize = floor.GetComponent<Renderer>().bounds.size;
        for (x = 1; x <= verticalSize-1; x++) // For para el alto 
        {
            for (z = 1; z <= horizontalSize-1; z++){ // For para el ancho
                Nodo n = new Nodo(x, 0, z,true,ref _grid,verticalSize,horizontalSize);
                nodeList.Add(n);
                _grid[x, z] =  n;
                Vector3 position = new Vector3(x * floorSize.x, 0, z * floorSize.z);

                // Instantiate(floor, new Vector3(x, 0, z),Quaternion.identity,parent.transform);

#if UNITY_EDITOR
                if (manualEditor)
                {
                   GameObject gb = Instantiate(floor, position, Quaternion.identity, parent.transform);
                   gb.GetComponent<SelectableNode>().SetNode(ref n);
                }
#endif
                // gb.hideFlags = HideFlags.NotEditable;
            }
           
            //Console.WriteLine(" ");
        }
        for (x = 1; x <= verticalSize-1; x++) // For para el alto 
        {
            for (z = 1; z <= horizontalSize - 1; z++)
            {
               _grid[x, z].CalculateConections();
               /*CalculateConnectionsJob calculateConnections =
                   new CalculateConnectionsJob(_grid[x, z]);
               calculateConnections.Schedule();*/
               //jobHandleArray.Append(calculateConections.Schedule());
            }
            
        }
       // JobHandle.CompleteAll(jobHandleArray);

        _grid[1,2].ShowConections();
        OnComplete?.Invoke();

        //RoomGenerator.Instance.GenerateRoom(_grid[horizontalSize/2,verticalSize/2]);
    }

    public GameObject InstantiatePrefab(GameObject gb, Nodo nodoWithPos)
    {
        if (floor==null)
        {
            floorSize = floor.GetComponent<Renderer>().bounds.size;
        }
        Vector3 position = new Vector3(nodoWithPos.x * floorSize.x, 0, nodoWithPos.z * floorSize.z);

        GameObject obectToSpawn = Instantiate(gb, position, Quaternion.identity);
        nodoWithPos.asociateGameObjectWithNode = obectToSpawn;
        NodeInfoToCollapse nodeInfo;
        if (nodoWithPos.asociateGameObjectWithNode.TryGetComponent<NodeInfoToCollapse>(out nodeInfo))
        {
            nodeInfo.SetNodo(nodoWithPos);
        }

        return obectToSpawn;
    } 
    public GameObject InstantiatePrefab(GameObject gb, Nodo nodoWithPos,Transform parentOfPrefab)
    {
        if (floor==null)
        {
            floorSize = floor.GetComponent<Renderer>().bounds.size;
        }
        Vector3 position = new Vector3(nodoWithPos.x * floorSize.x, 0, nodoWithPos.z * floorSize.z);
        GameObject obectToSpawn = Instantiate(gb, position, Quaternion.identity, parentOfPrefab);
        nodoWithPos.asociateGameObjectWithNode = obectToSpawn;
        NodeInfoToCollapse nodeInfo;
        if(nodoWithPos.asociateGameObjectWithNode.TryGetComponent<NodeInfoToCollapse>(out nodeInfo))
        {
            nodeInfo.SetNodo(nodoWithPos);
        }

        return obectToSpawn;
    }

    public static  (Nodo[,], List<Nodo>  nodeList) GenerateGridOfNeighbours(int ySize,int xSize,Vector3 tileSize,bool l_3d, TIleToCollapse allPosibilitiesToCollapse=null)
    {
        Nodo [,]  grid = new Nodo[ySize, xSize];
        List<Nodo>  nodeList= new List<Nodo>();

        // en 3d repartir en x z   en 2d y x

        int verticalIteration, horizontalIteration;

        NativeArray<JobHandle> jobHandleArray = new NativeArray<JobHandle>(ySize*xSize,Allocator.Persistent);
        for ( verticalIteration = 0; verticalIteration <= ySize-1; verticalIteration++) // For para el alto 
        {
            for (horizontalIteration = 0; horizontalIteration <= xSize-1; horizontalIteration++){ // For para el ancho
                
                //Nodo n = new Nodo(verticalIteration, 0, horizontalIteration,true,ref _grid,ySize,xSize);

                Nodo n = l_3d
                    ? new Nodo(verticalIteration, 0, horizontalIteration,true,ref grid,ySize,xSize)
                    : new Nodo(horizontalIteration,verticalIteration,0,false,ref grid,ySize,xSize);
                if (allPosibilitiesToCollapse != null)
                {
                    //n.TiIleToCollapse = allPosibilitiesToCollapse;
                    n.TiIleToCollapse = new TIleToCollapse();
                    n.TiIleToCollapse.AddNodo(n);
                    n.TiIleToCollapse.AddPosibilities(allPosibilitiesToCollapse.allPosibilitis);
                }
                nodeList.Add(n);
                grid[verticalIteration, horizontalIteration] =  n;
                Vector3 position = new Vector3(verticalIteration * tileSize.x, 0, horizontalIteration * tileSize.z);

                // Instantiate(floor, new Vector3(x, 0, z),Quaternion.identity,parent.transform);
                //GameObject gb = Instantiate(floor,position,Quaternion.identity,parent.transform);
                //gb.GetComponent<SelectableNode>().SetNode(ref n);
                // gb.hideFlags = HideFlags.NotEditable;
            }
           
            //Console.WriteLine(" ");
        }
        for (verticalIteration = 1; verticalIteration <= ySize-1; verticalIteration++) // For para el alto 
        {
            for (horizontalIteration = 1; horizontalIteration <= xSize - 1; horizontalIteration++)
            {
                grid[horizontalIteration, verticalIteration].CalculateConections();
                
            }
            
        }
        JobHandle.CompleteAll(jobHandleArray);
        return  (grid,nodeList);
    }
    public void DeleteGrid()
    {
        DestroyImmediate(parent); 
    }

    public Nodo GetNode(Vector2Int position)
    {
       // Debug.Log("position "+position+" horizonta "+horizontalSize+" vertical "+verticalSize);
        if (position.x < verticalSize && position.y < horizontalSize)
        {
            return Grid[position.x, position.y];
        }
        else return null;
    }
    public Nodo GetRandomNode() 
    {
        return nodeList[Random.Range(0, nodeList.Count)];
    }
    public void OnDrawGizmos()
    {
        /*if (nodeList != null)
        {
            foreach (var variableNodo in nodeList)
            {
                
                Gizmos.DrawCube(variableNodo.position,Vector3.one);
              
            }
        }
        */
    }
}
