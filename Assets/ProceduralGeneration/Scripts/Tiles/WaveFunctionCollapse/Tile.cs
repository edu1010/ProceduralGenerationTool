using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Nodo nodo;
    public bool isCollapsed = false;
    public bool canConnectUP,canConnectDown,canConnectLeft,canConnectRight;

  public GameObject[] possiblesConnectionsUP,
    possiblesConnectionsDown,
    possiblesConnectionsLeft,
    possiblesConnectionsRight;
}
