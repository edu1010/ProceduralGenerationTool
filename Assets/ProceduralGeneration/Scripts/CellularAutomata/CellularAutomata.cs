using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;
[ExecuteAlways]
public class CellularAutomata : MonoBehaviour
{
    int[,] _map;
    public int Width=40, Height=40;
    public int nSteps=1;
    public GameObject floor;
    public GameObject wall;
    public GameObject portal;
    bool portalCreated = false;

    [Range(0f, 1f)]
    public float FillRate= 0.545f;
    public int numberOfClusters => GetClusters().Count();
    private Color[] _randomColors ;
   public bool first = true;

    GameObject parent;
    GameObject tempGB;
    public bool doUpdate=false;
    public List<GameObject> posibleEvent=new List<GameObject>();
    public List<GameObject> portalEvent=new List<GameObject>();
    void instanciarMapa()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                Vector3 pos = new Vector3(i - Width / 2 + 0.5f, j - Height / 2 + 0.5f, 0);
                if (_map[i, j] == 1)
                {
                    tempGB= Instantiate(wall, pos, Quaternion.identity);
                }
                else
                {
                    tempGB = Instantiate(floor, pos, Quaternion.identity);
                    posibleEvent.Add(tempGB);
                    if (Random.Range(1, 10) > 5 && portalCreated == false)
                    {
                        GameObject p = Instantiate(portal, pos, Quaternion.identity);
                        portalCreated = true;
                        portalEvent.Add(p);
                        p.transform.parent = parent.transform;
                    }
                    else
                    {
                        if (j > Height - 2 && portalCreated == false)
                        {
                            GameObject p = Instantiate(portal, pos, Quaternion.identity);
                            portalCreated = true;
                            portalEvent.Add(p);
                            p.transform.parent = parent.transform;
                        }
                    }
                }
                tempGB.transform.parent = parent.transform;

            }
        }
    }

    public void InitAutomata()
    {
        
        SetRandomColors();
        FillMap();
        SmoothMap();
        FindClusters();

       // MergeClusters();
        //FindClusters();
        first = true;
        doUpdate = true;
        UpdateMap();

    }

    private void SetRandomColors()
    {
        _randomColors = new Color[128];
        for (int i = 0; i < _randomColors.Length; i++)
        {
            _randomColors[i] = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1);
        }
    }

    private void SmoothMap()
    {
        for (int i = 0; i < nSteps; i++)
        {
            DoSmoothstep();
        }
    }

    private void DoSmoothstep()
    {
        int[,] tempMap = new int[Width, Height];
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                tempMap[i, j] = GetsFilledAutomata(i, j) ? 1 : 0;
            }
        }
        _map = tempMap;
    }

    private bool GetsFilledAutomata(int i, int j)
    {
        int nNeighbours = GetNumberNeighbours(i, j);
        if (nNeighbours > 4)
            return true;
        if (nNeighbours < 4)
            return false;

        return _map[i, j] == 1;
    }

    private int GetNumberNeighbours(int i, int j)
    {
        int nNeighbours = 0;
        for (int x = i - 1; x <= i + 1; x++)
        {
            for (int y = j - 1; y <= j + 1; y++)
            {
                if (OutsideMap(x, y))
                    nNeighbours++;
                else if (_map[x, y] == 1)
                    nNeighbours++;

            }
        }
        return nNeighbours;
    }

    private bool OutsideMap(int x, int y)
    {
        return (x < 0 || x >= Width || y < 0 || y >= Height);
    }

    void FillMap()
    {
        _map = new int[Width, Height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                _map[x, y] = GetsFilled(x, y) ? 1 : 0;
            }
        }
    }

    private bool GetsFilled(int x, int y)
    {
        if (x == 0 || x == Width - 1 || y == 0 || y == Height - 1)
            return true;

        return Random.value < FillRate;
    }

    void FindClusters()
    {
        int currentClusterID = 2;
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (_map[i, j] == 0)
                {
                    FillCluster(i, j, currentClusterID++);
                }

            }
        }

    }

    private void FillCluster(int i, int j, int currentClusterID)
    {

        Queue<Tuple<int, int>> found = new Queue<Tuple<int, int>>();
        found.Enqueue(Tuple.Create(i, j));
        while (found.Count > 0)
        {
            var center = found.Dequeue();
            _map[center.Item1, center.Item2] = currentClusterID;
            //Top
            var check = Tuple.Create(center.Item1, center.Item2 + 1);
            if (_map[check.Item1, check.Item2] == 0 && !found.Contains(check))
                found.Enqueue(check);
            //Bottom
            check = Tuple.Create(center.Item1, center.Item2 - 1);
            if (_map[check.Item1, check.Item2] == 0 && !found.Contains(check))
                found.Enqueue(check);
            //Left
            check = Tuple.Create(center.Item1 - 1, center.Item2);
            if (_map[check.Item1, check.Item2] == 0 && !found.Contains(check))
                found.Enqueue(check);
            //Right
            check = Tuple.Create(center.Item1 + 1, center.Item2);
            if (_map[check.Item1, check.Item2] == 0 && !found.Contains(check))
                found.Enqueue(check);
        }
    }

    void MergeClusters()
    {
        List<int> clusters = GetClusters();
        if (clusters.Count > 1)
        {
            int c1 = clusters[Random.Range(0, clusters.Count)];
            clusters.Remove(c1);
            int c2 = clusters[Random.Range(0, clusters.Count)];
            ConnectClusters(c1, c2);
            clusters = GetClusters();
        }

    }

    private List<int> GetClusters()
    {
        List<int> clusters = new List<int>();
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {   
                if (!clusters.Contains(_map[i, j]) && _map[i, j] > 1)
                    clusters.Add(_map[i, j]);

            }
        }
        return clusters;
    }

    void ConnectClusters(int cluster1, int cluster2)
    {
        var current = GetRandomFromCluster(cluster1);
        var end = GetRandomFromCluster(cluster2);
        int clusterFound = cluster1;
        while (clusterFound == cluster1 || clusterFound == 1)
        {

            current = ConnectStep(current, end);
            clusterFound = _map[current.Item1, current.Item2];
            _map[current.Item1, current.Item2] = cluster1;
        }
        SetToCluster(clusterFound, cluster1);

    }


    private Tuple<int, int> GetRandomFromCluster(int clusterID)
    {
        List<Tuple<int, int>> found = new List<Tuple<int, int>>();
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (_map[i, j] == clusterID)
                    found.Add(Tuple.Create(i, j));

            }
        }
        int rdm = Random.Range(0, found.Count);
        return found[rdm];
    }

    private Tuple<int, int> ConnectStep(Tuple<int, int> current, Tuple<int, int> goal)
    {
        float[] probs = new float[4];

        //up
        probs[0] = Mathf.Max(0, goal.Item2 - current.Item2);
        //down
        probs[1] = Mathf.Max(0, current.Item2 - goal.Item2);
        //left
        probs[2] = Mathf.Max(0, current.Item1 - goal.Item1);
        //right
        probs[3] = Mathf.Max(0, goal.Item1 - current.Item1);

        float sum = probs.Sum();
        float rdm = Random.value;
        int i;
        float acumulativeBreakCondition = 0;
        for (i = 0; i < probs.Length; i++)
        {
            acumulativeBreakCondition += probs[i] / sum;
            if (rdm < acumulativeBreakCondition)
                break;
        }
        switch (i)
        {
            //up
            case 0:
                return Tuple.Create(current.Item1, current.Item2 + 1);
            //down
            case 1:
                return Tuple.Create(current.Item1, current.Item2 - 1);
            //left
            case 2:
                return Tuple.Create(current.Item1 - 1, current.Item2);
            //right
            case 3:
                return Tuple.Create(current.Item1 + 1, current.Item2);

            default:
                Debug.LogError("No step found");
                return Tuple.Create(current.Item1, current.Item2 + 1);

                break;
        }
    }

    private void SetToCluster(int oldID, int newID)
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (_map[i, j] == oldID)
                    _map[i, j] = newID;

            }
        }
    }
    public void CreateTilemap()
    {
        _map = new int[Width, Height];
        parent = new GameObject("Level");
       // instanciarMapa();
        first = false;
        InitAutomata();
    }
  

    public void UpdateMap()
    {

        while(doUpdate)
        {
            if (numberOfClusters > 1)
            {
                MergeClusters();
            }
            else
            {
                //Vector2 dondeDeboIr = GetComponent<DungeonGenerator>().ObtenerCuevaAbajo();
                if (first)
                {
                    parent = new GameObject();
                    instanciarMapa();
                    first = false;
                }
                else
                {
                    doUpdate=false;
                }

            }

        }
    }

    internal void DestroyTilemap()
    {
        DestroyImmediate(parent);
        first= true;
    }
}
