using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour 
{
    public GameObject MM;
    public GameObject MG;
    public GameObject C;
    public GameObject ST;

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void MiniGames()
    {
        MG.SetActive(true);
        MM.SetActive(false);
        C.SetActive(false);
    }

    public void Controls()
    {
        MG.SetActive(false);
        MM.SetActive(false);
        C.SetActive(true);
    }

    public void Back()
    {
        if (MG.activeSelf == true)
        {
            MG.SetActive(false);
            MM.SetActive(true);
            C.SetActive(false);
        }
        else if (C.activeSelf == true)
        {
            MG.SetActive(false);
            MM.SetActive(true);
            C.SetActive(false);
        }
        else if(ST.activeSelf == true)
        {
            ST.SetActive(false);
            C.SetActive(true);
            MM.SetActive(false);
            MG.SetActive(false);
        }
    }

    public void SpecialThanks()
    {
        ST.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
