
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[ExecuteInEditMode]
public class SelectableNode : MonoBehaviour
{
    private Renderer _renderer;
    public Nodo node;
    public static List<Nodo> nodesSelected = new List<Nodo>();
    MaterialPropertyBlock red ;
    MaterialPropertyBlock white;
    public bool changeColorsOnSelect = false;
    private void OnEnable()
    {
        _renderer = GetComponent<Renderer>();
        red =new MaterialPropertyBlock();
        white =new MaterialPropertyBlock();
        red.SetColor("_Color", Color.red);
        white.SetColor("_Color", Color.white);
    }

    public void SetNode(ref Nodo n)
    {
       // print("nodo asignado");
        node = n;
    }
    private bool checkCanRemove = false;
    private void Update()
    {
        if((!Selection.Contains(gameObject)) && !checkCanRemove){return;}
       // print("f");
        if (Selection.Contains(gameObject))
        {
            if (checkCanRemove){ return;}
            if(changeColorsOnSelect)
                _renderer.SetPropertyBlock(red);
           
            nodesSelected.Add(node);
            checkCanRemove = true;
           
        }
        else
        {
            if (!checkCanRemove){return;}

            if (changeColorsOnSelect)
                _renderer.SetPropertyBlock(white);
            
            nodesSelected.Remove(node);
            checkCanRemove = false;
            
        }
        
    }
}
#endif