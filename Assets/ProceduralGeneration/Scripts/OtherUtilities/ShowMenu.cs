using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowMenu : MonoBehaviour
{
    public GameObject MenuLanguage;
    bool isLanguageMenuActived = false;

    public GameObject Inventary;
    bool isInventaryMenuActived = false;

    public GameObject StartMenu;

    Vector3 visiblePosInventary;
    Vector3 invisiblePosInventary;

    private void Start()
    {
        visiblePosInventary = new Vector3(0.6f, 0.6f, 0.6f); 
        invisiblePosInventary = new Vector3(0, 0, 0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ActiveLanguageMenu();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            ActiveInventaryMenu();
        }
    }

    void ActiveLanguageMenu()
    {
        isLanguageMenuActived = !isLanguageMenuActived;
        MenuLanguage.SetActive(isLanguageMenuActived);
    }

    void ActiveInventaryMenu()
    {
        if (isInventaryMenuActived)
        {
            Inventary.transform.localScale = visiblePosInventary;
            isInventaryMenuActived = !isInventaryMenuActived;
        }
        else
        {
            Inventary.transform.localScale = invisiblePosInventary;
            isInventaryMenuActived = !isInventaryMenuActived;
        }
    }
    public void DesactivateStartMenu()
    {
        StartMenu.SetActive(false);
    }
}
