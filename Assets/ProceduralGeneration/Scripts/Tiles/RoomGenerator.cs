using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
public class RoomGenerator : MonoBehaviour
{
    //public CreateNecesaryRotations _createNecesaryRotations;
   [HideInInspector] public GridGenerator _gridGenerator;
    public static RoomGenerator Instance;
    //public int width  = 4;
    //public int height = 4;
    public GameObject CreatePrefab;
    
    private void OnEnable()
    {
        if (Instance != null && Instance != this) 
        { 
            DestroyImmediate(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    // Start is called before the first frame update
    public bool GenerateRandomRoom(int width,int height,GridGenerator gridGenerator)
    {
        _gridGenerator= gridGenerator;
      //  _createNecesaryRotations = GetComponent<CreateNecesaryRotations>();
      Nodo startPos =  _gridGenerator.GetRandomNode();
      return GenerateRoom(startPos, width, height);

    }

    public bool GenerateRoom(Nodo startPos,int width,int height)
    {
        List<Nodo> paredes= new List<Nodo>();
        GameObject parent = new GameObject("Room_");
        IndependentRoom independentRoom= parent.AddComponent<IndependentRoom>();
        //Calculamos paredes
        
        for (int x = startPos.x; x < width+startPos.x; x++)
        {
            Nodo up = _gridGenerator.GetNode(new Vector2Int(x, startPos.z));
            Nodo down = _gridGenerator.GetNode(new Vector2Int(x, height+startPos.z - 1));
            
            if (up == null || down == null) 
            { 
                DestroyImmediate(parent);
                return false;
            };
            if (up.isEmpty && down.isEmpty)
            {
                paredes.Add(up);
                paredes.Add(down);
                if (x == startPos.x || x ==width+startPos.x -1)
                {
                    independentRoom.AddCorner(up);
                    independentRoom.AddCorner(down);
                }
                else
                {
                    independentRoom.AddWall(up);
                    independentRoom.AddWall(down);
                }
                
            }else{DestroyImmediate(parent); return false;}
        }

        for (int z = startPos.z+1; z < height+startPos.z-1; z++)
        {
            Nodo left = _gridGenerator.GetNode(new Vector2Int(startPos.x, z));
            Nodo right = _gridGenerator.GetNode(new Vector2Int(width+startPos.x - 1, z));
            if (left == null || right == null) return false;
            if (left.isEmpty && right.isEmpty)
            {
                paredes.Add(left);
                paredes.Add(right);
               
                independentRoom.AddWall(left);
                independentRoom.AddWall(right);
            }else{ DestroyImmediate(parent); return false;}

        }

        foreach (var pared in paredes)//Compruebo que hay uno de distancia respecto al borde, para dejar espacio a los caminos
        {
            if (pared._conectionUP == false || pared._conectionDown == false || pared._conectionLeft == false || pared._conectionRight == false)
            {
                DestroyImmediate(parent);
                return false;
            }
        }

        for (int x = startPos.x + 1; x < width + startPos.x - 1; x++)
        {
            for (int z = startPos.z + 1; z < height + startPos.z - 1; z++)
            {
                Nodo floor = _gridGenerator.GetNode(new Vector2Int(x, z));
               
                independentRoom.AddFloor(floor);
                paredes.Add(floor);
            }
        }
        
        foreach (var pared in paredes)
        {
            pared.isEmpty = false;
            _gridGenerator.InstantiatePrefab(CreatePrefab, pared,parent.transform);
            
            pared.cost = 100;
        }
        
        foreach (var pared in paredes)
        {
            pared.asociateGameObjectWithNode.GetComponent<NodeInfoToCollapse>().Collapse();
        }
        return true;
        
    }

    public void GenerateSelectedRoom(GridGenerator gridGenerator,int width,int height)
    {

        _gridGenerator = gridGenerator;

#if UNITY_EDITOR
        //if (CheckAllTheNodesHaveConection())
        if (GenerateRoom(SelectableNode.nodesSelected[0], width, height))
        {
            print("VALID");
        }
        else
            print("NO VALID ROOM");
#endif
    }

#if UNITY_EDITOR
    public bool CheckAllTheNodesHaveConection()
    {
        List<Nodo> listnodes = SelectableNode.nodesSelected;
        foreach (Nodo nodo in listnodes)
        {
            foreach (var nodo2 in listnodes)
            {
                if (!nodo2.Equals(nodo))
                {
                    print("NodosCompatibles");
                    if (!nodo.conections.Contains(nodo2))
                    {
                        return false;
                    } 
                }
            }
           
        }

        if (SelectableNode.nodesSelected.Count < 4 )
            return false;
        return true;
    }

    public void CalculateStartPoint()
    {
        List<Nodo> listnodes = SelectableNode.nodesSelected;
        Nodo lowestNode = listnodes[0];

        foreach (Nodo nodo in listnodes)
        {
            if (nodo.x < lowestNode.x || (nodo.x == lowestNode.x && nodo.z < lowestNode.z))
            {
                lowestNode = nodo;
            }
        }
        GenerateRoom(lowestNode, 4, 4);
        

    }
#endif

}
