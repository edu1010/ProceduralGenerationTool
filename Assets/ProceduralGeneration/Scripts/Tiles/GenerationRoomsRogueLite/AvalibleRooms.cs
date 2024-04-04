 using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(menuName = "LevelPrefabs/AvalibleRooms")]
public class AvalibleRooms : ScriptableObject
{
    public GameObject[] Room_E;
    public GameObject[] Room_EN;
    public GameObject[] Room_ENS;
    public GameObject[] Room_ENST;
    public GameObject[] Room_ENSW;
    public GameObject[] Room_ENSWT;
    public GameObject[] Room_ENT;
    public GameObject[] Room_ENW;
    public GameObject[] Room_ENWT;
    public GameObject[] Room_ES;
    public GameObject[] Room_EST;
    public GameObject[] Room_ESW;
    public GameObject[] Room_ESWT;
    public GameObject[] Room_ET;
    public GameObject[] Room_EW;
    public GameObject[] Room_EWT;
    public GameObject[] Room_N;
    public GameObject[] Room_NS;
    public GameObject[] Room_NST;
    public GameObject[] Room_NSW;
    public GameObject[] Room_NSWT;
    public GameObject[] Room_NT;
    public GameObject[] Room_NW;
    public GameObject[] Room_NWT;
    public GameObject[] Room_S;
    public GameObject[] Room_ST;
    public GameObject[] Room_SW;
    public GameObject[] Room_SWT;
    public GameObject[] Room_W;
    public GameObject[] Room_WT;

    public GameObject GetRoom(bool n, bool s, bool e, bool w, bool t)
    {
        if (n)
        {
            if (s)
            {
                if (e)
                {
                    if (w)
                    {
                        if (t)
                        {
                            return Room_ENSWT[Random.Range(0, Room_ENSWT.Length)];
                        }
                        else
                        {
                            return Room_ENSW[Random.Range(0, Room_ENSW.Length)];
                        }
                    }
                    else
                    {
                        if (t)
                        {
                            return Room_ENST[Random.Range(0, Room_ENST.Length)];
                        }
                        else
                        {
                            return Room_ENS[Random.Range(0, Room_ENS.Length)];
                        }
                    }
                }
                else
                {
                    if (w)
                    {
                        if (t)
                        {
                            return Room_NSWT[Random.Range(0, Room_NSWT.Length)];
                        }
                        else
                        {
                            return Room_NSW[Random.Range(0, Room_NSW.Length)];
                        }
                    }
                    else
                    {
                        if (t)
                        {
                            return Room_NST[Random.Range(0, Room_NST.Length)];
                        }
                        else
                        {
                            return Room_NS[Random.Range(0, Room_NS.Length)];
                        }
                    }
                }
            }
            else
            {
                if (e)
                {
                    if (w)
                    {
                        if (t)
                        {
                            return Room_ENWT[Random.Range(0, Room_ENWT.Length)];
                        }
                        else
                        {
                            return Room_ENW[Random.Range(0, Room_ENW.Length)];
                        }
                    }
                    else
                    {
                        if (t)
                        {
                            return Room_ENT[Random.Range(0, Room_ENT.Length)];
                        }
                        else
                        {
                            return Room_EN[Random.Range(0, Room_EN.Length)];
                        }
                    }
                }
                else
                {
                    if (w)
                    {
                        if (t)
                        {
                            return Room_NWT[Random.Range(0, Room_NWT.Length)];
                        }
                        else
                        {
                            return Room_NW[Random.Range(0, Room_NW.Length)];
                        }
                    }
                    else
                    {
                        if (t)
                        {
                            return Room_NT[Random.Range(0, Room_NT.Length)];
                        }
                        else
                        {
                            return Room_N[Random.Range(0, Room_N.Length)];
                        }
                    }
                }
            }
        }
        else if (s)
        {
            if (e)
            {
                if (w)
                {
                    if (t)
                    {
                        return Room_ESWT[Random.Range(0, Room_ESWT.Length)];
                    }
                    else
                    {
                        return Room_ESW[Random.Range(0, Room_ESW.Length)];
                    }
                }
                else
                {
                    if (t)
                    {
                        return Room_EST[Random.Range(0, Room_EST.Length)];
                    }
                    else
                    {
                        return Room_ES[Random.Range(0, Room_ES.Length)];
                    }
                }
            }
            else
            {
                if (w)
                {
                    if (t)
                    {
                        return Room_SWT[Random.Range(0, Room_SWT.Length)];
                    }
                    else
                    {
                        return Room_SW[Random.Range(0, Room_SW.Length)];
                    }
                }
                else
                {
                    if (t)
                    {
                        return Room_ST[Random.Range(0, Room_ST.Length)];
                    }
                    else
                    {
                        return Room_S[Random.Range(0, Room_S.Length)];
                    }
                }
            }
        }
        else if (e)
        {
            if (w)
            {
                if (t)
                {
                    return Room_EWT[Random.Range(0, Room_EWT.Length)];
                }
                else
                {
                    return Room_EW[Random.Range(0, Room_EW.Length)];
                }
            }
            else
            {
                if (t)
                {
                    return Room_ET[Random.Range(0, Room_ET.Length)];
                }
                else
                {
                    return Room_E[Random.Range(0, Room_E.Length)];
                }
            }
        }
        else if (w)
        {
            if (t)
            {
                return Room_WT[Random.Range(0, Room_WT.Length)];
            }
            else
            {
                return Room_W[Random.Range(0, Room_W.Length)];
            }
        }

        return null;
    }

}
