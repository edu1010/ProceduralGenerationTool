using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelInvocator : MonoBehaviour
{
    [SerializeField] float timeToWait = 5f;
    [SerializeField] int levels = 10;
    [SerializeField] bool multiple = true;
    public void Awake()
    {
        DontDestroyOnLoad(gameObject);

    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        if(multiple)
        {
            for (int i = 0; i < levels; i++)
            {
                GameData.Instance.NextScene();
                yield return new WaitForSeconds(timeToWait);

            }
        }
        else
        {
            GameData.Instance.NextScene();
        }
        
    }
}
