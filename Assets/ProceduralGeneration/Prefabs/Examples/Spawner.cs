using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public float frequency = 1f;
    float elapsedTime = 0f;
    // Update is called once per frame
    void Update()
    {
        elapsedTime+=Time.deltaTime;
        if(elapsedTime>frequency)
        {
            elapsedTime = 0f;
            Instantiate(objectToSpawn);
        }
    }
}
