using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject MM;
    public GameObject MG;
    public GameObject C;
    public GameObject PM;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Resume();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7)) 
        {
            Debug.Log("Esc was pressed");
            if (GameIsPaused)
            {
                Resume();

                if (MM.activeSelf)
                {
                    MM.SetActive(false);
                }

                if (C.activeSelf)
                {
                    C.SetActive(false);
                }

                if (MG.activeSelf)
                {
                    MG.SetActive(false);
                }
            } else
            {
                Paused();
            }
        }
    }

    public void Resume()
    {
        PM.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Paused()
    {
        PM.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMainMenu()
    {
        Cursor.visible = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
        Debug.Log("Loading Menu...");
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
}
