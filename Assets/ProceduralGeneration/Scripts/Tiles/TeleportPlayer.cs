using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    public Transform nextLocation;
    public Transform pointOfAparition;

    private void OnEnable()
    {
        if(pointOfAparition=null)
        {
            pointOfAparition = transform;
            pointOfAparition.position += Vector3.one;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            other.gameObject.SetActive(false);
            other.transform.position= nextLocation.position;
            other.gameObject.SetActive(true);
        }
    }
}
