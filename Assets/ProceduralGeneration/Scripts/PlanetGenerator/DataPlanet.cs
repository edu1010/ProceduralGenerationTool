using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu(fileName = "PlanetTest", menuName = "ScriptableObjects/PlanetTest", order = 1)]
public class DataPlanet : ScriptableObject
{
  public ComputeShader planetShader;
  public float _testValue;

  public float TestValue
  {
    get => _testValue;
    set { _testValue = value; 
      planetShader.SetFloat("testValue",value);}
  }

}
