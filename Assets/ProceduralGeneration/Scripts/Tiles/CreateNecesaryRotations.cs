using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNecesaryRotations : MonoBehaviour
{
    private CornerRotationTypes _whatRotationIHave;
   [SerializeField] private GameObject _startPrefab;
    public GameObject CreatePrefabInTheCorrectRotation(CornerRotationTypes desireRotation)
    {
        GameObject go = _startPrefab;
        switch (desireRotation)
        {
            case CornerRotationTypes.UpLookingLeft:
                go.transform.rotation = Quaternion.Euler(0,0,0);
                break;
            case CornerRotationTypes.UpLookingRight:
                go.transform.rotation = Quaternion.Euler(0,-90,0);
                break;
            case CornerRotationTypes.DownLookingRight:
                go.transform.rotation = Quaternion.Euler(0,-180,0);
                break;
            case CornerRotationTypes.DownLookingLeft:
                go.transform.rotation = Quaternion.Euler(0,90,0);
                break;
        }

        return go;
    }
    public static  GameObject CreatePrefabInTheCorrectRotation(GameObject go,CornerRotationTypes desireRotation)
    {
        switch (desireRotation)
        {
            case CornerRotationTypes.UpLookingLeft:
                go.transform.rotation = Quaternion.Euler(0,0,0);
                break;
            case CornerRotationTypes.UpLookingRight:
                go.transform.rotation = Quaternion.Euler(0,90,0);
                break;
            case CornerRotationTypes.DownLookingRight:
                go.transform.rotation = Quaternion.Euler(0,-180,0);
                break;
            case CornerRotationTypes.DownLookingLeft:
                go.transform.rotation = Quaternion.Euler(0,-90,0);
                break;
        }

        return go;
    }
}

public enum CornerRotationTypes
{//si la base es up lockingleft
    UpLookingRight,//-90 A
    UpLookingLeft,// B
    DownLookingRight,//-180 D
    DownLookingLeft// +90 grados en Y C
}

/*
 A= Es arriba mirando derecha
 B= Es Arriba mirando izquierda
 D= Es abajo mirnaod derecha
 C= Es abajo mirando izquierda
 A****************B
 *                *
 *                *
 *                *
 *                *
 *                *
 *                *
 D****************C
 */
 