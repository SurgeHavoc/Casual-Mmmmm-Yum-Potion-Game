using UnityEngine;

public class RtPInstructions : MonoBehaviour
{
    public GameObject instruct;
    public PauseScript pause;
    public RubThePotionGame rub;
    public void Intructions()
    {
        instruct.SetActive(false);
        Time.timeScale = 1f;
        pause.GameIsPaused = false;
        rub.IsTimerRunning = true;
    }
}
