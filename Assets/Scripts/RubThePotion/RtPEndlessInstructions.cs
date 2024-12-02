using UnityEngine;

public class RtPEndlessInstructions : MonoBehaviour
{
    public GameObject instruct;
    public PauseScript pause;
    public RubThePotionGameEndless rub;
    public void Intructions()
    {
        instruct.SetActive(false);
        Time.timeScale = 1f;
        pause.GameIsPaused = false;
        rub.IsTimerRunning = true;
    }
}
