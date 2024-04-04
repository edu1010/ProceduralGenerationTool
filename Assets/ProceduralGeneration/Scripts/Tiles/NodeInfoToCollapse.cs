using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class NodeInfoToCollapse : MonoBehaviour
{
    [SerializeField] TypeOfNodes _myType =TypeOfNodes.Empty;
    [SerializeField] LevelPrefabs    _levelPrefabs;
    private Nodo _nodo;
    [SerializeField]MeshRenderer _MeshRenderer;
    GameObject _goToSpawn;
    private void Awake()
    {
        if (_levelPrefabs == null)
        {
            _levelPrefabs = Resources.Load<LevelPrefabs>("New Level Prefabs");
        }
    }
    public TypeOfNodes GetType()
    {
        return _myType;
    }
    public void SetNodo(Nodo nodo)
    {
        _nodo= nodo;

        _nodo.nodeData = this;
    }
    public TypeOfNodes GetNodoType()
    {
       return _myType;
    }
    public void Inicialize(TypeOfNodes type,Nodo nodoToAsign)
    {
        _nodo = nodoToAsign;
        _myType= type;
    }
    public void ForceCollapse(TypeOfNodes type)
    {
        _myType = type;
        DestroyImmediate(_goToSpawn);
        _MeshRenderer.enabled = false;
        CalculateConectionsAvalibles();
        CreateGameObject();
    }
    public void Collapse()
    {
        CalculateTypeOfNode();
        _MeshRenderer.enabled= false;
        CreateGameObject();


    }
    int _currentRecolapseTimes= 0;
    int maxRecolapseInfoTimes = 4;
    public void ReCollapse()
    {
        _currentRecolapseTimes++;
        if (_currentRecolapseTimes >= maxRecolapseInfoTimes) return;
        DestroyImmediate(_goToSpawn);
        _MeshRenderer.enabled = false;
        CalculateConectionsAvalibles();
        CreateGameObject();
    }
    public void CreateGameObject()
    {
        _nodo.isEmpty= false;
        if (_goToSpawn != null)
        {
            DestroyImmediate(_goToSpawn);
        }
        switch (_myType)
        {
            case TypeOfNodes.Floor:
                _goToSpawn = Instantiate(_levelPrefabs.GetRandomFloor(), transform);
                _goToSpawn.transform.localPosition = Vector3.zero;
                break;
            case TypeOfNodes.Wall:
                _goToSpawn = Instantiate(_levelPrefabs.GetRandomWall(), transform);
                _goToSpawn.transform.localPosition = Vector3.zero;
                if (!m_Down)
                {
                    CreateNecesaryRotations.CreatePrefabInTheCorrectRotation(_goToSpawn, CornerRotationTypes.UpLookingLeft);
                }
                else if (!m_UP)
                {
                    CreateNecesaryRotations.CreatePrefabInTheCorrectRotation(_goToSpawn, CornerRotationTypes.DownLookingRight);
                }
                else if (!m_left)
                {
                    CreateNecesaryRotations.CreatePrefabInTheCorrectRotation(_goToSpawn, CornerRotationTypes.UpLookingRight);

                }
                else
                {
                    CreateNecesaryRotations.CreatePrefabInTheCorrectRotation(_goToSpawn, CornerRotationTypes.DownLookingLeft);

                }

                break;
            case TypeOfNodes.Corner:
                _goToSpawn = Instantiate(_levelPrefabs.GetRandomCorner(), transform);
                _goToSpawn.transform.localPosition = Vector3.zero;

                if (m_Down && m_right)
                    CreateNecesaryRotations.CreatePrefabInTheCorrectRotation(_goToSpawn, CornerRotationTypes.DownLookingRight);
                if (m_Down && m_left)
                    CreateNecesaryRotations.CreatePrefabInTheCorrectRotation(_goToSpawn, CornerRotationTypes.DownLookingLeft);
                if (m_UP && m_left)
                {
                    CreateNecesaryRotations.CreatePrefabInTheCorrectRotation(_goToSpawn, CornerRotationTypes.UpLookingLeft);
                }
                if (m_UP && m_right)
                {
                    CreateNecesaryRotations.CreatePrefabInTheCorrectRotation(_goToSpawn, CornerRotationTypes.UpLookingRight);
                }
                break;
            case TypeOfNodes.Door:
                _goToSpawn = Instantiate(_levelPrefabs.GetRandomDoor(), transform);
                _goToSpawn.transform.localPosition = Vector3.zero;
                if(m_left&& m_right)
                {
                    if(m_UP)
                        CreateNecesaryRotations.CreatePrefabInTheCorrectRotation(_goToSpawn, CornerRotationTypes.UpLookingLeft);
                    else
                        CreateNecesaryRotations.CreatePrefabInTheCorrectRotation(_goToSpawn, CornerRotationTypes.DownLookingRight);

                }
                if (m_UP && m_Down)
                {
                    if(m_right)
                        CreateNecesaryRotations.CreatePrefabInTheCorrectRotation(_goToSpawn, CornerRotationTypes.UpLookingRight);
                    else
                        CreateNecesaryRotations.CreatePrefabInTheCorrectRotation(_goToSpawn, CornerRotationTypes.DownLookingLeft);

                }
                break;
            case TypeOfNodes.Corridor:

                PropagateInfo();
                
                #region Filtrado
                
                if (m_UP)
                {
                    if(_nodo.nodoUP.nodeData._myType != TypeOfNodes.Corridor && _nodo.nodoUP.nodeData._myType != TypeOfNodes.CorridorCorner && _nodo.nodoUP.nodeData._myType != TypeOfNodes.Door)
                    {
                        m_UP = false;
                    }
                }
                if (m_Down)
                {
                    if(_nodo.nodoDown.nodeData._myType != TypeOfNodes.Corridor && _nodo.nodoDown.nodeData._myType != TypeOfNodes.CorridorCorner && _nodo.nodoDown.nodeData._myType != TypeOfNodes.Door)
                    {
                        m_Down = false;
                    }
                }
                if (m_right)
                {
                    if (_nodo.nodoRight.nodeData._myType != TypeOfNodes.Corridor && _nodo.nodoRight.nodeData._myType != TypeOfNodes.CorridorCorner && _nodo.nodoRight.nodeData._myType != TypeOfNodes.Door)
                    {
                        m_right = false;
                    }
                }
                if (m_left)
                {
                    if (_nodo.nodoLeft.nodeData._myType != TypeOfNodes.Corridor && _nodo.nodoLeft.nodeData._myType != TypeOfNodes.CorridorCorner && _nodo.nodoLeft.nodeData._myType != TypeOfNodes.Door)
                    {
                        m_left = false;
                    }
                }
                #endregion
               
                if (m_UP && m_Down)
                {
                    _goToSpawn = Instantiate(_levelPrefabs.GetRandomCorridor(), transform);
                    _goToSpawn.transform.localPosition = Vector3.zero;
                    CreateNecesaryRotations.CreatePrefabInTheCorrectRotation(_goToSpawn, CornerRotationTypes.DownLookingLeft);
                }else if(m_left&& m_right)
                {
                    _goToSpawn = Instantiate(_levelPrefabs.GetRandomCorridor(), transform);
                    _goToSpawn.transform.localPosition = Vector3.zero;
                }
                else
                {
                    _goToSpawn = Instantiate(_levelPrefabs.GetRandomCorridorCorner(), transform);
                    _goToSpawn.transform.localPosition = Vector3.zero;


                    if (m_left) 
                    {
                        if (m_UP)
                        {
                            CreateNecesaryRotations.CreatePrefabInTheCorrectRotation(_goToSpawn, CornerRotationTypes.UpLookingLeft);
                        }
                        if (m_Down)
                        {
                            CreateNecesaryRotations.CreatePrefabInTheCorrectRotation(_goToSpawn, CornerRotationTypes.DownLookingLeft);
                        }
                        
                    
                    }
                    if(m_right) 
                    {
                        if (m_UP)
                        {
                            CreateNecesaryRotations.CreatePrefabInTheCorrectRotation(_goToSpawn, CornerRotationTypes.UpLookingRight);

                        }
                        if (m_Down)
                        {
                            CreateNecesaryRotations.CreatePrefabInTheCorrectRotation(_goToSpawn, CornerRotationTypes.DownLookingRight);

                        }
                    }
                }
                break;
            case TypeOfNodes.Empty:
                break;
            
        }
    }

    bool m_Down = false, m_left = false, m_right = false,m_UP=false;
    public void CalculateConectionsAvalibles()
    {
        if (_nodo._conectionDown)
        {
            if (!_nodo.nodoDown.isEmpty)
            {
                m_Down = true;
            }
        }
        if (_nodo._conectionLeft)
        {
            if (!_nodo.nodoLeft.isEmpty)
            {
                m_left = true;
            }
        }
        if (_nodo._conectionRight)
        {
            if (!_nodo.nodoRight.isEmpty)
            {
                m_right = true;
            }
        }
        if (_nodo._conectionUP)
        {
            if (!_nodo.nodoUP.isEmpty)
            {
                m_UP = true;
            }
        }
    }
    public void CalculateTypeOfNode()
    {
        if (_myType == TypeOfNodes.Door) return;
        CalculateConectionsAvalibles();
        //Los nodos de los pasillos son los últimos en calcularse, y todos deben conectar con otro pasillo o puerta unicamente
         if (m_left)
        {
           if( _nodo.nodoLeft.nodeData.GetNodoType() == TypeOfNodes.Corridor ||
                _nodo.nodoLeft.nodeData.GetNodoType() == TypeOfNodes.Door)
            {
                _myType = TypeOfNodes.Corridor;
            }
        }
         if (m_right)
        {
            if(_nodo.nodoRight.nodeData.GetNodoType()== TypeOfNodes.Corridor ||
                _nodo.nodoRight.nodeData.GetNodoType() == TypeOfNodes.Door)
            {
                _myType = TypeOfNodes.Corridor;
            }
        }
        if (m_UP)
        {
            if(_nodo.nodoUP.nodeData.GetNodoType()== TypeOfNodes.Corridor ||
                _nodo.nodoUP.nodeData.GetNodoType() == TypeOfNodes.Door)
            {
                _myType = TypeOfNodes.Corridor;
            }
        }
        if (m_Down)
        {
            if(_nodo.nodoDown.nodeData.GetNodoType()== TypeOfNodes.Corridor ||
                _nodo.nodoDown.nodeData.GetNodoType() == TypeOfNodes.Door)
            {
                _myType = TypeOfNodes.Corridor;
            }
        }
        if (_myType == TypeOfNodes.Corridor) return;
        //Nodos de las salas
        if (m_left && m_right && m_UP && m_Down)
            _myType = TypeOfNodes.Floor;
        else if ((m_left || m_right) && (m_UP || m_Down))
        {
            _myType = TypeOfNodes.Corner;
        }
        if (((m_left && m_right) || (m_UP && m_Down))&& _myType != TypeOfNodes.Floor)
        {
            _myType = TypeOfNodes.Wall;
        }
        gameObject.name = _myType.ToString();
    }
    private bool _infoProPagated = false;
    public void PropagateInfo()
    {
        if (_infoProPagated) return;
        _infoProPagated = true;
        if (_myType == TypeOfNodes.Corridor || _myType == TypeOfNodes.CorridorCorner)
        {
            //pasar que soy corridor para volver a mirar tipo de corridor

            if(m_Down && (_nodo.nodoDown.nodeData._myType==TypeOfNodes.Corridor || _nodo.nodoDown.nodeData._myType == TypeOfNodes.CorridorCorner))
            {
                _nodo.nodoDown.nodeData.ReCollapse();
            }
            if (m_left && (_nodo.nodoLeft.nodeData._myType == TypeOfNodes.Corridor || _nodo.nodoLeft.nodeData._myType == TypeOfNodes.CorridorCorner))
            {
                _nodo.nodoLeft.nodeData.ReCollapse();
            }
            if (m_right && (_nodo.nodoRight.nodeData._myType == TypeOfNodes.Corridor || _nodo.nodoRight.nodeData._myType == TypeOfNodes.CorridorCorner))
            {
                _nodo.nodoRight.nodeData.ReCollapse();
            }
            if (m_UP && (_nodo.nodoUP.nodeData._myType == TypeOfNodes.Corridor || _nodo.nodoUP.nodeData._myType == TypeOfNodes.CorridorCorner))
            {
                _nodo.nodoUP.nodeData.ReCollapse();
            }
        } 
    }

    public void Reset()
    {
        _myType =TypeOfNodes.Empty;
        _MeshRenderer.enabled = false;
        DestroyImmediate(_goToSpawn);
    }
}
public enum TypeOfNodes
{
    Floor,
    Wall,
    Corner,
    Door,
    Corridor,
    CorridorCorner,
    Empty
}
