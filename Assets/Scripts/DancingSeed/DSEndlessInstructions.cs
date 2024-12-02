using UnityEngine;

public class DSEndlessInstructions : MonoBehaviour
{
    public GameObject instruct;
    public PauseScript pause;
    public DancingSeedGameEndless seed;

    public void Intructions()
    {
        instruct.SetActive(false);
        Time.timeScale = 1f;
        pause.GameIsPaused = false;
        // Begin the countdown on game start here.
        seed.StartCountdown(); 
    }

}
