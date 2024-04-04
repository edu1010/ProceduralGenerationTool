using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TIleToCollapse : MonoBehaviour
{
    private Nodo _nodo;

    public Tile[] allPosibilitis;
    public void AddNodo(Nodo node)
    {
        _nodo=node;
    }
    public void AddPosibilities(Tile[] tiles)
    {
        allPosibilitis = tiles;
    }
    public void Collapse(Nodo n)
    {
        _nodo = n;
        Tile my = allPosibilitis[Random.Range(0, allPosibilitis.Length)];
        my.isCollapsed = true;
        allPosibilitis = null;
        allPosibilitis=new Tile[]{my};
        Instantiate(my, _nodo.position, Quaternion.identity);
    }
    public void Collapse()
    {
        Tile my = allPosibilitis[Random.Range(0, allPosibilitis.Length)];
        my.isCollapsed = true;
        allPosibilitis = null;
        allPosibilitis=new Tile[]{my};
        Instantiate(my, _nodo.position, Quaternion.identity);
    }
}
/*When using our to analyze the input image,
 we also need to record the frequency at which each of its tiles appears. 
 We will later use these numbers as weights when deciding which square’s wavefunction to collapse,
  and when choosing which tile to assign to a square when it is being collapsed.
public void CalculateWeight(float weight)
{
    float shannon_entropy_for_square = Mathf.Log(Mathf.sum(weight)) -
        (sum(weight * log(weight)) / sum(weight))
}*/