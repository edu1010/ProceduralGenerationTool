using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public class Model : MonoBehaviour
{
    Nodo[,] _grid;
    private List<Nodo> _nodeList;
    public int verticalSize;
    public int horizontalSize;
    Vector3 tileSize = Vector3.one;
    public int dimension = 2;
    public TIleToCollapse tileToCollapse;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        (_grid, _nodeList) = GridGenerator.GenerateGridOfNeighbours(verticalSize, horizontalSize, tileSize, true, tileToCollapse);
        for (int y = 0; y < verticalSize; y++)
        {
            for (int x = 0; x < horizontalSize; x++)
            {
                if (_grid[y, x].isEmpty)
                {
                    List<Nodo> listaOrdenada = CalcularEntropia();
                    Colapse(listaOrdenada);
                }
                else
                {
                    //print
                }
            }
        }
    }
    //CalculateNestTileToColapse()

    public void CalculateNextTileToColapse() 
    {
         
    }
    public List<Nodo> CalcularEntropia()
    {
        List<Nodo> listaOrdenada = _nodeList.Where(x => x.isEmpty == true).ToList();

        listaOrdenada.Sort((a, b) => a.TiIleToCollapse.allPosibilitis.Length - b.TiIleToCollapse.allPosibilitis.Length);//ordenar por la que tienen menos opciones

        //Dejar solo las que tengan menos entropia para coger una aleatoria
        listaOrdenada = listaOrdenada.Where(x => listaOrdenada[0].TiIleToCollapse.allPosibilitis.Length.Equals(x.TiIleToCollapse.allPosibilitis.Length)).ToList();
        return listaOrdenada;

    }

    public void Colapse(List<Nodo> nodosOrderByEntropy)
    {
        int r = Random.Range(0, nodosOrderByEntropy.Count-1);
        nodosOrderByEntropy[r].TiIleToCollapse.Collapse(nodosOrderByEntropy[r]);

        nodosOrderByEntropy[r].isEmpty=false;
    }
}
