using UnityEngine;

public class InstructionsEndless : MonoBehaviour
{
    public GameObject instruct;
    public PauseScript pause;
    public GameManagerEndless pti;

    public void Intructions()
    {
        instruct.SetActive(false);
        Time.timeScale = 1f;
        pause.GameIsPaused = false;
    }
}
