using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseScript : MonoBehaviour
{
    public bool GameIsPaused = false;

    public GameObject MG;
    public GameObject C;
    public GameObject PM;

    public GameObject instruct;
    public GameManager GM;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Resume();
    }

    public void Resume()
    {
        PM.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Paused()
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
        C.SetActive(false);
    }

    public void Controls()
    {
        MG.SetActive(false);
        C.SetActive(true);
    }

    public void Intructions()
    {
        instruct.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        GM.IsTimerRunning = true;
    }

    public void BackToPause()
    {
        MG.SetActive(false);
        C.SetActive(false);
        PM.SetActive(true);
    }
}
