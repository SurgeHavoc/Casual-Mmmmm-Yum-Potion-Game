using UnityEngine;

public class Instructions : MonoBehaviour
{
    public GameObject instruct;
    public PauseScript pause;
    public GameManager pti;

    public void Intructions()
    {
        instruct.SetActive(false);
        Time.timeScale = 1f;
        pause.GameIsPaused = false;
    }
}
